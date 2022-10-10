using AutoMapper;
using Microsoft.EntityFrameworkCore;
using smrpo_be.Data;
using smrpo_be.Data.Models;
using smrpo_be.Data.Requests.Sprint;
using smrpo_be.Data.WebModels;
using smrpo_be.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace smrpo_be.Services
{
    public interface ISprintService
    {
        IEnumerable<SprintDto> GetProjectSprints(Guid projectId);
        SprintDto Create(SprintCreate model);
        SprintDto Update(Guid id, SprintUpdate model);
        void Delete(Guid Id);
        IEnumerable<UserStoryDto> GetUserStories(Guid id);
        IEnumerable<UserStoryDto> AddUserStory(Guid id, Guid userStoryId);
    }


    public class SprintService : ISprintService
    {
        private readonly SmrpoContext db;
        private readonly IMapper map;
        private readonly IAuthenticationService _authService;

        public SprintService(SmrpoContext context, IMapper mapper, IAuthenticationService authService)
        {
            db = context;
            map = mapper;
            _authService = authService;
        }

        public IEnumerable<SprintDto> GetProjectSprints(Guid projectId)
        {
            User currentUser = _authService.CurrentUser();

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject userProject = db.UserProjects.Find(currentUser.Id, projectId);
                if (userProject == null) throw new AppException("User is not a part of the project");
            }

            IEnumerable<Sprint> sprints = db.Sprints
                .Include(x => x.UserStories)
                .ThenInclude(x => x.UserStoryTimes)
                .Where(x => x.Project.Id == projectId)
                .OrderBy(x => x.Start);

            return map.Map<IEnumerable<SprintDto>>(sprints);
        }

        public SprintDto Create(SprintCreate model)
        {
            User currentUser = _authService.CurrentUser();

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject userProject = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == model.ProjectId);
                if (userProject == null) throw new AppException("User is not a part of the project");
                if (!userProject.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("User is not Scrum Master");
            }

            if (model.End <= model.Start) throw new AppException("End date before start date");
            if (model.Start.Date < DateTime.Now.Date) throw new AppException("Start date is in the past");
            if (model.Velocity <= 0) throw new AppException("Velocity must be greater than 0");
            if (model.Velocity > 1000) throw new AppException("Velocity must be less than 1000");

            //bool overlap = tStartA < tEndB && tStartB < tEndA;
            IEnumerable<Sprint> overlapping = db.Sprints
                .Where(x => x.Project.Id == model.ProjectId)
                .Where(x => x.Start <= model.End.Date && model.Start.Date <= x.End);

            if (overlapping.Count() > 0) throw new AppException($"Must not overlap. {overlapping.First().Start.ToString("MMM dd, yyyy")} - {overlapping.First().End.ToString("MMM dd, yyyy")}");

            Sprint sprint = map.Map<Sprint>(model);
            sprint.Project = db.Projects.Find(model.ProjectId);

            db.Sprints.Add(sprint);

            //add UserStoryTime to all existing user stories
            IEnumerable<UserStory> userStories = db.UserStories.Where(x => x.ProjectId == model.ProjectId);
            foreach (UserStory userStory in userStories)
            {
                db.UserStoryTimes.Add(new UserStoryTime
                {
                    Estimation = 0,
                    UserStory = userStory,
                    Sprint = sprint
                });
            }

            db.SaveChanges();

            return map.Map<SprintDto>(sprint);
        }

        public SprintDto Update(Guid id, SprintUpdate model)
        {
            User currentUser = _authService.CurrentUser();

            Sprint sprint = db.Sprints
                .Include(x => x.UserStories)
                .Include(x => x.Project)
                .FirstOrDefault(x => x.Id == id);

            if (sprint == null) throw new AppException("Could not find the sprint");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject userProject = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == sprint.ProjectId);
                if (userProject == null) throw new AppException("User is not a part of the project");
                if (!userProject.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("User is not Scrum Master");
            }

            if (model.Velocity <= 0) throw new AppException("Velocity must be greater than 0");
            if (model.Velocity > 1000) throw new AppException("Velocity must be less than 1000");

            if (sprint.Start <= DateTime.Now && sprint.End >= DateTime.Now)
            {
                int userStoriesEstimationSum = db.UserStoryTimes
                .Include(x => x.UserStory)
                .Where(x => x.UserStory.SprintId == id)
                .Sum(x => x.Estimation);

                if (userStoriesEstimationSum > model.Velocity) throw new AppException("Velocity can not be less than current cumulative estimation of user stories which is " + userStoriesEstimationSum);

                sprint.Velocity = model.Velocity;

                db.Update(sprint);
                db.SaveChanges();

                return map.Map<SprintDto>(sprint);
            }
            else
            {
                if (model.End <= model.Start) throw new AppException("End date before start date");
                if (model.Start < DateTime.Today) throw new AppException("Start date is in the past");

                IEnumerable<Sprint> overlapping = db.Sprints
                    .Where(x => x.Project.Id == sprint.ProjectId)
                    .Where(x => x.Start < model.End.Date && model.Start.Date < x.End)
                    .Where(x => x.Id != sprint.Id);

                if (overlapping.Count() > 0) throw new AppException($"Must not overlap. {overlapping.First().Start.ToString("MMM dd, yyyy")} - {overlapping.First().End.ToString("MMM dd, yyyy")}");

                sprint.Start = model.Start;
                sprint.End = model.End;
                sprint.Velocity = model.Velocity;

                db.Update(sprint);
                db.SaveChanges();

                return map.Map<SprintDto>(sprint);
            }
        }
        public void Delete(Guid id)
        {
            User currentUser = _authService.CurrentUser();

            Sprint sprint = db.Sprints
                .Include(x => x.UserStories)
                .Include(x => x.Project)
                .FirstOrDefault(x => x.Id == id);

            if (sprint == null) throw new AppException("Could not find the sprint");
            if (sprint.Start <= DateTime.Now && sprint.End >= DateTime.Now) throw new AppException("Sprint cannot be removed if active");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject userProject = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == sprint.ProjectId);
                if (userProject == null) throw new AppException("User is not a part of the project");
                if (!userProject.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("User is not Scrum Master");
            }

            db.Remove(sprint);
            db.SaveChanges();
        }

        public IEnumerable<UserStoryDto> GetUserStories(Guid id)
        {
            User currentUser = _authService.CurrentUser();

            Sprint sprint = db.Sprints
                .Include(x => x.Project)
                .Include(x => x.UserStories)
                .ThenInclude(x => x.AcceptanceTests)
                .FirstOrDefault(x => x.Id == id);

            if (sprint == null) throw new AppException("Could not find the sprint");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject userProject = db.UserProjects.Find(currentUser.Id, sprint.Project.Id);
                if (userProject == null) throw new AppException("User is not a part of the project");
            }

            return map.Map<IEnumerable<UserStoryDto>>(sprint.UserStories);
        }

        public IEnumerable<UserStoryDto> AddUserStory(Guid id, Guid userStoryId)
        {
            User currentUser = _authService.CurrentUser();

            Sprint sprint = db.Sprints
                .Include(x => x.UserStories)
                .Include(x => x.Project)
                .FirstOrDefault(x => x.Id == id);

            if (sprint == null) throw new AppException("Could not find the sprint");

            UserStory existingStory = sprint.UserStories.FirstOrDefault(x => x.Id == userStoryId);
            if (existingStory != null) throw new AppException("User story is already part of the sprint");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject userProject = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == sprint.Project.Id);
                if (userProject == null) throw new AppException("User is not a part of the project");
                if (!userProject.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("User is not Scrum Master");
            }

            UserStory userStory = db.UserStories.Include(x => x.Sprint).FirstOrDefault(x => x.Id == userStoryId);
            if (userStory == null) throw new AppException("Could not find the user story");
            if (sprint.Project.Id != userStory.Project.Id) throw new AppException("Sprint and user story are not part of the same project");
            if (userStory.Status == Data.Enums.UserStoryStatus.Finished) throw new AppException("User story is already finished");

            UserStoryTime userStoryTime = db.UserStoryTimes.Find(userStoryId, id);
            if (userStoryTime.Estimation == 0) throw new AppException("User story is not estimated");

            if (userStory.Sprint != null && userStory.Sprint.Active) throw new AppException("User story is currently inside active sprint");

            int userStoriesEstimationSum = db.UserStoryTimes
                .Include(x => x.UserStory)
                .Where(x => x.UserStory.SprintId == id)
                .Sum(x => x.Estimation);

            if (userStoriesEstimationSum + userStoryTime.Estimation > sprint.Velocity) throw new AppException("Addition of this story to the active sprint exceeds its velocity. Current cumulative estimation is " + userStoriesEstimationSum + " and velocity is " + sprint.Velocity);

            userStory.Sprint = sprint;
            db.Update(userStory);
            db.SaveChanges();

            return GetUserStories(id);
        }
    }
}
