using AutoMapper;
using Microsoft.EntityFrameworkCore;
using smrpo_be.Data;
using smrpo_be.Data.Models;
using smrpo_be.Data.Requests.UserStory;
using smrpo_be.Data.WebModels;
using smrpo_be.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace smrpo_be.Services
{
    public interface IUserStoryService
    {
        IEnumerable<UserStoryDto> GetProjectUserStories(Guid projectId);
        UserStoryDto Create(UserStoryCreate model);
        void Delete(Guid id);
        UserStoryDto Update(Guid id, UserStoryUpdate model);
        IEnumerable<UserStoryTimeDto> GetUserStoryTimes(Guid id);
        UserStoryTimeDto UpdateUserStoryTime(Guid id, UserStoryTimeUpdate model);
        UserStoryDto Accept(Guid id);
        UserStoryDto Reject(Guid id, UserStoryDecline model);
    }

    public class UserStoryService : IUserStoryService
    {
        private readonly SmrpoContext db;
        private readonly IMapper map;
        private readonly IAuthenticationService _authService;


        public UserStoryService(SmrpoContext context, IMapper mapper, IAuthenticationService authService)
        {
            db = context;
            map = mapper;
            _authService = authService;
        }

        public IEnumerable<UserStoryDto> GetProjectUserStories(Guid projectId)
        {
            User currentUser = _authService.CurrentUser();

            Project project = db.Projects.Find(projectId);
            if (project == null) throw new AppException("Project not found");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects.Find(currentUser.Id, projectId);
                if (up == null) throw new AppException("User must be part of the project");
            }

            IEnumerable<UserStory> userStories = db.UserStories
                .Include(x => x.AcceptanceTests)
                .Include(x => x.Sprint)
                .Where(x => x.ProjectId == project.Id && !x.Deleted);

            return map.Map<IEnumerable<UserStoryDto>>(userStories);
        }

        public UserStoryDto Create(UserStoryCreate model)
        {
            User currentUser = _authService.CurrentUser();

            Project project = db.Projects.Find(model.ProjectId);
            if (project == null) throw new AppException("Project not found");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == model.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x != Data.Enums.ProjectRole.Developer).Any()) throw new AppException("User must be project's Scrum master or Product owner to edit project user stories");
            }

            if (string.IsNullOrWhiteSpace(model.Name)) throw new AppException("Name is required");
            if (model.BusinessValue <= 0) throw new AppException("Bussiness value must be greater than 0");
            if (model.BusinessValue > 10) throw new AppException("Bussiness value must be less than 10");

            if (model.AcceptanceTests == null || model.AcceptanceTests.Count() <= 0)
                throw new AppException("Acceptance test is required");

            if (db.UserStories.Where(x => x.ProjectId == project.Id && x.Name == model.Name && !x.Deleted).Count() > 0) throw new AppException("Story with given name already exists for this project");

            UserStory userStory = map.Map<UserStory>(model);

            db.Add(userStory);

            //add UserStoryTime to for existing sprints
            IEnumerable<Sprint> sprints = db.Sprints.Where(x => x.ProjectId == model.ProjectId);
            foreach (Sprint sprint in sprints)
            {
                db.UserStoryTimes.Add(new UserStoryTime
                {
                    Estimation = 0,
                    UserStory = userStory,
                    Sprint = sprint
                });
            }

            db.SaveChanges();

            return map.Map<UserStoryDto>(userStory);
        }

        public void Delete(Guid id)
        {
            User currentUser = _authService.CurrentUser();

            UserStory userStory = db.UserStories.Include(x => x.Sprint).FirstOrDefault(x => x.Id == id);
            if (userStory == null) throw new AppException("User story does not exist");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects.Find(currentUser.Id, userStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x != Data.Enums.ProjectRole.Developer).Any()) throw new AppException("User must be project's Scrum master or Product owner to delete user story");
            }

            if (userStory.Status == Data.Enums.UserStoryStatus.Finished) throw new AppException("User story is already finished");
            if (userStory.Sprint != null) throw new AppException("User story is already part of a sprint");

            userStory.Deleted = true;
            db.Update(userStory);
            db.SaveChanges();
        }

        public UserStoryDto Update(Guid id, UserStoryUpdate model)
        {
            User currentUser = _authService.CurrentUser();

            UserStory userStory = db.UserStories.Include(x => x.Sprint).Include(x => x.AcceptanceTests).FirstOrDefault(x => x.Id == id);
            if (userStory == null) throw new AppException("User story does not exist");

            Project project = db.Projects.Find(userStory.ProjectId);
            if (project == null) throw new AppException("Project not found");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects.Find(currentUser.Id, userStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x != Data.Enums.ProjectRole.Developer).Any()) throw new AppException("User must be project's Scrum master or Product owner to delete user story");
            }

            if (userStory.Status == Data.Enums.UserStoryStatus.Finished) throw new AppException("User story is already finished");
            if (userStory.Sprint != null) throw new AppException("User story is already part of a sprint");

            if (string.IsNullOrWhiteSpace(model.Name)) throw new AppException("Name is required");
            if (model.BusinessValue <= 0) throw new AppException("Bussiness value must be greater than 0");
            if (model.BusinessValue > 10) throw new AppException("Bussiness value must be less than 10");

            if (model.AcceptanceTests == null || model.AcceptanceTests.Count() <= 0)
                throw new AppException("Acceptance test is required");

            if (db.UserStories.Where(x => x.ProjectId == project.Id && x.Name == model.Name && !x.Deleted && x.Name != userStory.Name).Count() > 0) throw new AppException("Story with given name already exists for this project");

            UserStory userStoryModel = map.Map<UserStory>(model);
            userStory.Name = userStoryModel.Name;
            userStory.Description = userStoryModel.Description;
            userStory.Priority = userStoryModel.Priority;
            foreach(AcceptanceTest accpetanceTest in userStory.AcceptanceTests)
            {
                db.Remove(accpetanceTest);
            }
            userStory.AcceptanceTests = userStoryModel.AcceptanceTests;
            userStory.BusinessValue = userStoryModel.BusinessValue;

            db.Update(userStory);
            db.SaveChanges();

            return map.Map<UserStoryDto>(userStory);
        }

        public IEnumerable<UserStoryTimeDto> GetUserStoryTimes(Guid id)
        {
            User currentUser = _authService.CurrentUser();

            UserStory userStory = db.UserStories.Find(id);
            if (userStory == null) throw new AppException("User story does not exist");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects.Find(currentUser.Id, userStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
            }

            IEnumerable<UserStoryTime> userStoryTimes = db.UserStoryTimes
                .Include(x => x.Sprint)
                .Where(x => x.UserStoryId == id);

            return map.Map<IEnumerable<UserStoryTimeDto>>(userStoryTimes);
        }

        public UserStoryTimeDto UpdateUserStoryTime(Guid id, UserStoryTimeUpdate model)
        {
            if (model.SprintId == null) throw new AppException("SprintId is required");

            User currentUser = _authService.CurrentUser();

            UserStoryTime userStoryTime = db.UserStoryTimes.Find(id, model.SprintId);
            if (userStoryTime == null) throw new AppException("User story time does not exist");

            UserStory userStory = db.UserStories.Include(x => x.Sprint).First(x => x.Id == id);
            if (userStory == null) throw new AppException("User story does not exist");

            Sprint sprint = db.Sprints.FirstOrDefault(x => x.Id == model.SprintId);
            if (sprint == null) throw new AppException("Could not find the sprint");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == userStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("User must be project's Scrum master to edit user story time");
            }

            if (userStory.Status == Data.Enums.UserStoryStatus.Finished) throw new AppException("User story is already finished");
            if (userStory.Sprint != null) throw new AppException("User story is already assigned to a sprint");

            userStoryTime.Estimation = model.Estimation;
            db.Update(userStoryTime);
            db.SaveChanges();

            return map.Map<UserStoryTimeDto>(userStoryTime);
        }

        public UserStoryDto Accept(Guid id)
        {
            User currentUser = _authService.CurrentUser();

            UserStory userStory = db.UserStories
                .Include(x => x.Sprint)
                .Include(x => x.Tasks)
                .FirstOrDefault(x => x.Id == id);
            if (userStory == null) throw new AppException("User story not found");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == userStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.ProductOwner).Any()) throw new AppException("User must be project's Product owner to accept user story");
            }

            if (userStory.Sprint == null) throw new AppException("User story is not a part of a sprint");
            //if (!userStory.Sprint.Active) throw new AppException("User story is not a part of an active sprint");
            if (userStory.Tasks.Count() <= 0) throw new AppException("User story has no tasks");

            if (userStory.Tasks.Where(x => x.RemainingTime > 0).Any()) throw new AppException("User story is not finished yet");

            if (userStory.Status == Data.Enums.UserStoryStatus.Finished) throw new AppException("User story is already finished");

            userStory.Status = Data.Enums.UserStoryStatus.Finished;
            db.Update(userStory);
            db.SaveChanges();

            return map.Map<UserStoryDto>(userStory);
        }

        public UserStoryDto Reject(Guid id, UserStoryDecline model)
        {
            User currentUser = _authService.CurrentUser();

            UserStory userStory = db.UserStories
                .Include(x => x.Sprint)
                .Include(x => x.Tasks)
                .FirstOrDefault(x => x.Id == id);
            if (userStory == null) throw new AppException("User story not found");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == userStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.ProductOwner).Any()) throw new AppException("User must be project's Product owner to reject user story");
            }

            if (string.IsNullOrWhiteSpace(model.Comment)) throw new AppException("Reject reason is required");

            if (userStory.Sprint == null) throw new AppException("User story is not a part of a sprint");
            //if (!userStory.Sprint.Active) throw new AppException("User story is not a part of an active sprint");

            if (userStory.Status == Data.Enums.UserStoryStatus.Finished) throw new AppException("User story is already finished");

            userStory.Status = Data.Enums.UserStoryStatus.Unfinished;
            userStory.Comment = model.Comment;
            userStory.Sprint = null;
            userStory.SprintId = null;
            db.Update(userStory);
            db.SaveChanges();

            return map.Map<UserStoryDto>(userStory);
        }

    }
}
