using AutoMapper;
using Microsoft.EntityFrameworkCore;
using smrpo_be.Data;
using smrpo_be.Data.Enums;
using smrpo_be.Data.Models;
using smrpo_be.Data.Requests.Project;
using smrpo_be.Data.WebModels;
using smrpo_be.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace smrpo_be.Services
{
    public interface IProjectService
    {
        IEnumerable<ProjectDto> GetAll();
        ProjectDto Get(Guid projectId);
        IEnumerable<ProjectPostDto> GetProjectPosts(Guid projectId);
        IEnumerable<ProjectUserDto> GetProjectUsers(Guid projectId);
        ProjectDto Create(ProjectCreate model);
        IEnumerable<ProjectUserDto> AddUser(Guid projectId, ProjectAddUser model);
        IEnumerable<ProjectUserDto> RemoveRole(Guid projectId, Guid userId, ProjectRole role);
        IEnumerable<ProjectPostDto> AddPost(Guid projectId, ProjectAddPost model);
        IEnumerable<ProjectUserDto> RemoveUser(Guid projectId, Guid userId);
        void Delete(Guid projectId);
        ProjectDto EditProject(Guid projectId, ProjectEdit model);
    }

    public class ProjectService : IProjectService
    {
        private readonly SmrpoContext db;
        private readonly IMapper map;
        private readonly IAuthenticationService _authService;


        public ProjectService(SmrpoContext context, IMapper mapper, IAuthenticationService authService)
        {
            db = context;
            map = mapper;
            _authService = authService;
        }

        public IEnumerable<ProjectDto> GetAll()
        {
            User currentUser = _authService.CurrentUser();

            // If not admin return only their projects
            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                return GetUserProjects(currentUser.Id);
            }

            IEnumerable<Project> projects = db.Projects.Include(x => x.Users).ThenInclude(x => x.ProjectRoles).Include(x => x.Users).ThenInclude(x => x.User);

            return map.Map<IEnumerable<ProjectDto>>(projects);

        }

        public ProjectDto Get(Guid projectId)
        {
            Project project = db.Projects.Find(projectId);
            if (project == null) throw new AppException("Project not found");

            User currentUser = _authService.CurrentUser();

            ProjectDto userProject = map.Map<ProjectDto>(project);
            

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == projectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (up != null)
                    userProject.ProjectRoles = up.Roles;
            }

            return userProject;
        }

        public IEnumerable<ProjectPostDto> GetProjectPosts(Guid projectId)
        {
            Project project = db.Projects.Find(projectId);
            if (project == null) throw new AppException("Project not found");

            User currentUser = _authService.CurrentUser();


            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == projectId);
                if (up == null) throw new AppException("User must be part of the project");
            }

            IEnumerable<ProjectPost> projectPosts = db.ProjectPosts
                                                        .Include(x => x.User)
                                                        .Where(x => x.ProjectId == projectId);

            return map.Map<IEnumerable<ProjectPostDto>>(projectPosts); ;
        }

        public IEnumerable<ProjectDto> GetUserProjects(Guid userId)
        {
            IEnumerable<UserProject> projects = db.UserProjects
                .Include(x => x.ProjectRoles)
                .Include(x => x.Project)
                    .ThenInclude(x => x.Users)
                    .ThenInclude(x => x.User)
                .Include(x => x.Project)
                    .ThenInclude(x => x.Users)
                    .ThenInclude(x => x.ProjectRoles)
                .Where(x => x.UserId == userId);

            return map.Map<IEnumerable<ProjectDto>>(projects);
        }

        public IEnumerable<ProjectUserDto> GetProjectUsers(Guid projectId)
        {
            User currentUser = _authService.CurrentUser();
            IEnumerable<UserProject> userProjects = db.UserProjects
                .Include(x => x.ProjectRoles)
                .Include(x => x.User)
                .Where(x => x.ProjectId == projectId);

            // Must be admin or part of the project
            if (!(currentUser.Role == Data.Enums.UserRole.Administrator ||
                userProjects.Where(x => x.UserId == currentUser.Id).Any())) throw new AppException("User not part of the project");

            return map.Map<IEnumerable<ProjectUserDto>>(userProjects);
        }

        public ProjectDto Create(ProjectCreate model)
        {
            User currentUser = _authService.CurrentUser();
            if (currentUser.Role != Data.Enums.UserRole.Administrator) throw new AppException("Must be admin to create project");

            if (string.IsNullOrWhiteSpace(model.Name)) throw new AppException("Project name is required");

            if (db.Projects.Where(x => x.Name == model.Name).Any()) throw new AppException("Project name must be unique");

            Project project = map.Map<Project>(model);
            db.Projects.Add(project);
            db.SaveChanges();

            if(model.ProjectUsers != null && model.ProjectUsers.Count() > 0)
            {
                foreach(ProjectAddUser projectAddUser in model.ProjectUsers)
                {
                    AddUser(project.Id, projectAddUser);
                }
            }

            return map.Map<ProjectDto>(project);
        }

        public ProjectDto EditProject(Guid projectId, ProjectEdit model)
        {
            User currentUser = _authService.CurrentUser();

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                // If user is not an admin, check if he is project scrum master and not editing themselves
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == projectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("User must be project's Scrum master to edit other users");
            }

            if (string.IsNullOrWhiteSpace(model.Name)) throw new AppException("Project name is required");

            if (db.Projects.Where(x => x.Id != projectId && x.Name == model.Name).Any()) 
                throw new AppException("Project name must be unique");

            
            Project project = db.Projects.Where(x => x.Id == projectId).Include(x => x.Users).ThenInclude(x => x.ProjectRoles).Include(x => x.Users).ThenInclude(x => x.User).FirstOrDefault();
            if (project == null) throw new AppException("Could not find project");

            if (model.ProjectUsers != null)
            {
                foreach (UserProject user in project.Users.ToList())
                {
                    //for each role of user in project
                    foreach (UserProjectRole userRole in db.UserProjectRole.Where(y => y.UserProject.UserId == user.UserId && y.UserProject.ProjectId == projectId).ToList())
                    {

                        if (!model.ProjectUsers.Where(x => x.Id == user.UserId && x.ProjectRole == userRole.Role).Any())
                        {
                            RemoveRole(projectId, user.UserId, userRole.Role);
                        }
                    }
                    if (!model.ProjectUsers.Where(x => x.Id == user.UserId).Any())
                    {
                        RemoveUser(project.Id, user.UserId);
                    }
                }
                foreach (ProjectAddUser projectAddUser in model.ProjectUsers)
                {
                    AddUser(project.Id, projectAddUser);
                }
            }

            project.Name = model.Name;
            db.Projects.Update(project);
            db.SaveChanges();

            return Get(projectId);
        }

        public IEnumerable<ProjectUserDto> AddUser(Guid projectId, ProjectAddUser model)
        {
            User currentUser = _authService.CurrentUser();

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                // If user is not an admin, check if he is project scrum master and not editing themselves
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == projectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("User must be project's Scrum master to edit other users");
                //if (model.Id == currentUser.Id) throw new AppException("Cannot edit own role");
            }

            IEnumerable<UserProject> userProjects = db.UserProjects
                .Include(x => x.ProjectRoles)
                .Where(x => x.ProjectId == projectId);
            foreach (var up in userProjects)
            {
                if (model.ProjectRole == Data.Enums.ProjectRole.ScrumMaster)
                {
                    if (up.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any() && up.UserId != model.Id) {
                        throw new AppException("There can be only one Scrum master");
                    }
                }

                if (model.ProjectRole == Data.Enums.ProjectRole.ProductOwner)
                {
                    if (up.Roles.Where(x => x == Data.Enums.ProjectRole.ProductOwner).Any() && up.UserId != model.Id)
                    {
                        throw new AppException("There can be only one Product owner");
                    }
                }
            }


            UserProject userProject = db.UserProjects
                .Include(x => x.ProjectRoles)
                .FirstOrDefault(x => x.UserId == model.Id && x.ProjectId == projectId);

            // If user is already a part of project, only update their role
            if (userProject != null)
            {
                if (!userProject.Roles.Where(x => x == model.ProjectRole).Any())
                {
                    UserProjectRole role = new UserProjectRole()
                    {
                        Role = model.ProjectRole,
                        UserProject = userProject
                    };

                    db.Add(role);
                }
            }
            else
            {
                Project project = db.Projects.Find(projectId);
                if (project == null) throw new AppException("Project not found");

                User user = db.Users.Find(model.Id);
                if (user == null) throw new AppException("User not found");

                userProject = new UserProject()
                {
                    Project = project,
                    User = user,
                    ProjectRoles = new List<UserProjectRole>() { new UserProjectRole { Role = model.ProjectRole } }
                };
                db.UserProjects.Add(userProject);
            }
            db.SaveChanges();

            return GetProjectUsers(projectId);
        }

        public IEnumerable<ProjectUserDto> RemoveRole(Guid projectId, Guid userId, ProjectRole role)
        {
            UserProjectRole projectRole = db.UserProjectRole
                .FirstOrDefault(x => x.UserProject.UserId == userId && x.UserProject.ProjectId == projectId && x.Role == role);
            if (projectRole == null) throw new AppException("Project role not found");

            User currentUser = _authService.CurrentUser();
            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                // If user is not an admin, check if he is project scrum master and not removing themselves
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == projectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("User must be project's Scrum master to remove other users");
                if (userId == currentUser.Id && role == ProjectRole.ScrumMaster) throw new AppException("Cannot self-remove Scrum master role, please contact admin");
            }

            if(role == ProjectRole.Developer)
            {
                IEnumerable<UserStoryTask> tasks = db.Tasks.Where(x => x.UserId == userId).ToList();
                foreach(UserStoryTask task in tasks)
                {
                    task.UserId = null;
                    task.Accepted = false;
                    task.Status = UserStoryTaskStatus.Unassigned;
                    db.Tasks.Update(task);
                }
            }

            db.UserProjectRole.Remove(projectRole);
            db.SaveChanges();

            return GetProjectUsers(projectId);
        }

        public IEnumerable<ProjectPostDto> AddPost(Guid projectId, ProjectAddPost model)
        {
            Project project = db.Projects.Find(projectId);
            if (project == null) throw new AppException("Project not found");
            if (model.Title == null) throw new AppException("Post title is mandatory parameter");

            User currentUser = _authService.CurrentUser();

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == projectId);
                if (up == null) throw new AppException("User must be part of the project");
            }

            ProjectPost projectPost = map.Map<ProjectPost>(model);
            projectPost.User = currentUser;
            projectPost.Project = project;

            db.ProjectPosts.Add(projectPost);
            db.SaveChanges();

            return GetProjectPosts(projectId);
        }

        public IEnumerable<ProjectUserDto> RemoveUser(Guid projectId, Guid userId)
        {
            UserProject userProject = db.UserProjects.Find(userId, projectId);
            if (userProject == null) throw new AppException("Users project not found");

            User currentUser = _authService.CurrentUser();
            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                // If user is not an admin, check if he is project scrum master and not removing themselves
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == projectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("User must be project's Scrum master to remove other users");
                if (userId == currentUser.Id) throw new AppException("Cannot self-remove");
            }

            IEnumerable<UserStoryTask> tasks = db.Tasks.Where(x => x.UserId == userId).ToList();
            foreach (UserStoryTask task in tasks)
            {
                task.UserId = null;
                task.Accepted = false;
                task.Status = UserStoryTaskStatus.Unassigned;
                db.Tasks.Update(task);
            }

            db.UserProjects.Remove(userProject);
            db.SaveChanges();

            return GetProjectUsers(projectId);
        }

        public void Delete(Guid projectId)
        {
            Project project = db.Projects.Find(projectId);
            if (project == null) throw new AppException("Project not found");

            User currentUser = _authService.CurrentUser();
            if (currentUser.Role != Data.Enums.UserRole.Administrator) throw new AppException("Must be admin to remove projects");

            db.Projects.Remove(project);
            db.SaveChanges();
        }
    }
}
