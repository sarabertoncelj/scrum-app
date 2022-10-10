using AutoMapper;
using Microsoft.EntityFrameworkCore;
using smrpo_be.Data;
using smrpo_be.Data.Models;
using smrpo_be.Data.Requests;
using smrpo_be.Data.Requests.UserStory;
using smrpo_be.Data.WebModels;
using smrpo_be.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;


namespace smrpo_be.Services
{
    public interface ITaskService
    {
        IEnumerable<StoryTaskDto> GetStoryTasks(Guid storyId);
        IEnumerable<StoryTaskDto> CreateStoryTask(TaskCreate model);
        StoryTaskDto AssignTask(Guid id, Guid userId);
        StoryTaskDto AcceptTask(Guid id);
        StoryTaskDto DeclineTask(Guid id);
        StoryTaskDto FinishTask(Guid id);
        IEnumerable<StoryTaskDto> EditTask(Guid taskId, TaskEdit model);
        IEnumerable<StoryTaskDto> DeleteTask(Guid taskId);
        IEnumerable<WorkLogDto> GetTaskWorkLogs(Guid taskId);
        StoryTaskDto StartWork(Guid taskId);
        StoryTaskDto EndWork(Guid taskId);
        IEnumerable<WorkLogDto> UpdateWorkLog(Guid logId, UpdateWorkLog model);
        IEnumerable<WorkLogDto> CreateWorkLog(Guid taskId, CreateWorkLog model);
    }

    public class TaskService : ITaskService
    {
        private readonly SmrpoContext db;
        private readonly IMapper map;
        private readonly IAuthenticationService _authService;


        public TaskService(SmrpoContext context, IMapper mapper, IAuthenticationService authService)
        {
            db = context;
            map = mapper;
            _authService = authService;
        }

        public IEnumerable<StoryTaskDto> GetStoryTasks(Guid storyId)
        {
            User currentUser = _authService.CurrentUser();

            UserStory userStory = db.UserStories
                .Include(x => x.Tasks)
                .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.Id == storyId);
            if (userStory == null) throw new AppException("User story doesnt exist");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects.Find(currentUser.Id, userStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
            }

            return map.Map<IEnumerable<StoryTaskDto>>(userStory.Tasks);
        }

        public IEnumerable<StoryTaskDto> CreateStoryTask(TaskCreate model)
        {
            User currentUser = _authService.CurrentUser();

            UserStory userStory = db.UserStories
                .Include(x => x.Sprint)
                .FirstOrDefault(x => x.Id == model.UserStoryId);
            if (userStory == null) throw new AppException("User story doesnt exist");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == userStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x != Data.Enums.ProjectRole.ProductOwner).Any())
                    throw new AppException("User must be project's Scrum master or Developer to add tasks");
            }

            if (userStory.Status == Data.Enums.UserStoryStatus.Finished) throw new AppException("User story is already finished");

            if (userStory.Sprint != null && !userStory.Sprint.Active) throw new AppException("Can only add tasks to user stories that are inside active sprint");

            if (string.IsNullOrWhiteSpace(model.Description)) throw new AppException("Description is required");

            if (model.RemainingTime <= 0) throw new AppException("Time estimate must be greater than 0");
            if (model.RemainingTime > 1000) throw new AppException("Time estimate must be lower than 1000");

            if (model.UserId.HasValue)
            {
                UserProject up = db.UserProjects
                .Include(x => x.ProjectRoles)
                .FirstOrDefault(x => x.UserId == model.UserId && x.ProjectId == userStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.Developer).Any()) throw new AppException("User must be Developer to be assigned a task");

                UserProject upcurrent = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == userStory.ProjectId);
                //If user is NOT scrum master and IS developer
                if (!upcurrent.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any() && upcurrent.Roles.Where(x => x == Data.Enums.ProjectRole.Developer).Any())
                {
                    if (currentUser.Id != model.UserId) throw new AppException("Developers can only assign tasks to themselves");
                }
            }

            UserStoryTask task = map.Map<UserStoryTask>(model);

            if(model.UserId != null)
                task.Status = Data.Enums.UserStoryTaskStatus.Assigned;
            if (model.UserId == currentUser.Id)
                task.Accepted = true;

            db.Add(task);
            db.SaveChanges();

            return GetStoryTasks(model.UserStoryId);
        }

        public StoryTaskDto AssignTask(Guid id, Guid userId)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .FirstOrDefault(x => x.Id == id);
            if (task == null) throw new AppException("Task doesn't exist");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject uproj = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == task.UserStory.ProjectId);
                if (uproj == null) throw new AppException("User must be part of the project");
                if (!uproj.Roles.Where(x => x != Data.Enums.ProjectRole.ProductOwner).Any())
                    throw new AppException("User must be project's Scrum master or Developer to assign user");
            }


            UserProject up = db.UserProjects
                .Include(x => x.ProjectRoles)
                .FirstOrDefault(x => x.UserId == userId && x.ProjectId == task.UserStory.ProjectId);
            if (up == null) throw new AppException("User must be part of the project");
            if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.Developer).Any()) throw new AppException("User must be Developer to be assigned a task");

            UserProject upcurrent = db.UserProjects
                .Include(x => x.ProjectRoles)
                .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == task.UserStory.ProjectId);
            //If user is NOT scrum master and IS developer
            if (upcurrent != null && !upcurrent.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any() && upcurrent.Roles.Where(x => x == Data.Enums.ProjectRole.Developer).Any())
            {
                if (currentUser.Id != userId) throw new AppException("Developers can only assign tasks to themselves");
            }

            if (task.Accepted) throw new AppException("Task has already been accepted");

            task.Status = Data.Enums.UserStoryTaskStatus.Assigned;
            task.User = db.Users.Find(userId);
            if (userId == currentUser.Id)
                task.Accepted = true;

            db.Update(task);
            db.SaveChanges();

            return map.Map<StoryTaskDto>(task);
        }

        public StoryTaskDto AcceptTask(Guid id)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .FirstOrDefault(x => x.Id == id);
            if (task == null) throw new AppException("Task doesnt exist");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects.Find(currentUser.Id, task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
            }

            if (task.Accepted) throw new AppException("Task has already been accepted");

            if (!task.UserId.HasValue) throw new AppException("User must be assigned before accepting the task");
            if (task.UserId != currentUser.Id) throw new AppException("Only assigned user can accept the task");

            task.Accepted = true;
            db.Update(task);
            db.SaveChanges();

            return map.Map<StoryTaskDto>(task);
        }

        public StoryTaskDto DeclineTask(Guid id)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .FirstOrDefault(x => x.Id == id);
            if (task == null) throw new AppException("Task doesnt exist");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (task.UserId != currentUser.Id && !up.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()) throw new AppException("Only assigned user or scrum master can decline the task");
            }

            if (task.Status == Data.Enums.UserStoryTaskStatus.Finished) throw new AppException("Task is already finished");

            if (!task.UserId.HasValue) throw new AppException("User must be assigned before declining the task");

            task.Accepted = false;
            task.UserId = null;
            task.User = null;
            task.Status = Data.Enums.UserStoryTaskStatus.Unassigned;

            db.Update(task);
            db.SaveChanges();

            return map.Map<StoryTaskDto>(task);
        }

        public StoryTaskDto FinishTask(Guid id)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .FirstOrDefault(x => x.Id == id);
            if (task == null) throw new AppException("Task doesnt exist");

            if (task.UserId != currentUser.Id) throw new AppException("Only assigned user can finish the task");
            if (!task.Accepted) throw new AppException("Task must be accepted first");

            if (task.UserStory.Status == Data.Enums.UserStoryStatus.Finished) throw new AppException("Cannot complete task of a finished story");

            if (task.Status == Data.Enums.UserStoryTaskStatus.Finished) throw new AppException("Task is already finished");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects.Find(currentUser.Id, task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
            }

            task.Status = Data.Enums.UserStoryTaskStatus.Finished;
            task.RemainingTime = 0;

            db.Update(task);
            db.SaveChanges();

            return map.Map<StoryTaskDto>(task);
        }


        public IEnumerable<StoryTaskDto> EditTask(Guid taskId, TaskEdit model)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .FirstOrDefault(x => x.Id == taskId);

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x != Data.Enums.ProjectRole.ProductOwner).Any())
                    throw new AppException("User must be Developer or Scrum master to edit tasks");
                // If user is developer, and the task has been accepted, only the assigned user can edit the task
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any()
                    && task.Accepted
                    && task.UserId.Value != currentUser.Id)
                    throw new AppException("Only assigned developers can edit tasks");
            }

            task.Description = model.Description;
            task.RemainingTime = model.RemainingTime;

            if(task.RemainingTime == 0)
            {
                task.Status = Data.Enums.UserStoryTaskStatus.Finished;
            } else
            {
                task.Status = Data.Enums.UserStoryTaskStatus.Assigned;
            }

            db.Tasks.Update(task);
            db.SaveChanges();

            return GetStoryTasks(task.UserStoryId);
        }

        public IEnumerable<StoryTaskDto> DeleteTask(Guid taskId)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .FirstOrDefault(x => x.Id == taskId);

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x != Data.Enums.ProjectRole.ProductOwner).Any())
                    throw new AppException("User must be Developer or Scrum master to delete tasks");
            }
            if (task.Accepted) throw new AppException("Accepted tasks cannot be deleted");

            Guid storyId = task.UserStoryId;
            db.Tasks.Remove(task);
            db.SaveChanges();

            return GetStoryTasks(storyId);
        }

        public IEnumerable<WorkLogDto> GetTaskWorkLogs(Guid taskId)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .ThenInclude(x => x.Sprint)
                .FirstOrDefault(x => x.Id == taskId);

            bool canSeeAllLogs = true;

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up= db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x != Data.Enums.ProjectRole.ProductOwner).Any())
                    throw new AppException("User must be Developer or Scrum master to see work logs");

                // If not scrum master, they can only see their own logs
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.ScrumMaster).Any())
                    canSeeAllLogs = false;
            }

            IEnumerable<WorkLog> workLogs = null;

            if (canSeeAllLogs)
                workLogs = db.WorkLogs.Where(x => x.TaskId == taskId).OrderBy(x => x.Day);
            else
                workLogs = db.WorkLogs.Where(x => x.TaskId == taskId && x.UserId == currentUser.Id).OrderBy(x => x.Day);

            if (!workLogs.Any() && currentUser.Id == task.UserId)
            {
                CreateWorkLog model = new CreateWorkLog();
                model.Day = DateTime.Now;
                model.UserId = currentUser.Id;
                model.HoursRemaining = task.RemainingTime;
                model.HoursWorked = 0;
                CreateWorkLog(task.Id, model);

                if (canSeeAllLogs)
                    workLogs = db.WorkLogs.Where(x => x.TaskId == taskId).OrderBy(x => x.Day);
                else
                    workLogs = db.WorkLogs.Where(x => x.TaskId == taskId && x.UserId == currentUser.Id).OrderBy(x => x.Day);
            }

            //Create missing work logs if in active sprint
            Sprint sprint = task.UserStory.Sprint;
            if (sprint.Start <= DateTime.Now && sprint.End >= DateTime.Now)
            {
                IEnumerable<Guid> distinctUserIds = workLogs.Select(x => x.UserId).Distinct().ToList();
                foreach (Guid distinctUserId in distinctUserIds)
                {
                    foreach (DateTime day in EachDay(sprint.Start.Date, DateTime.Now).Where(x => !workLogs.Where(y => y.Day.Date == x.Date && y.UserId == distinctUserId).Any()))
                    {
                        CreateWorkLog model = new CreateWorkLog();
                        model.Day = day;
                        model.UserId = distinctUserId;
                        model.HoursRemaining = day == DateTime.Now.Date ? task.RemainingTime : 0;
                        model.HoursWorked = 0;
                        CreateWorkLog(task.Id, model);
                    }
                }
            }

            if (canSeeAllLogs)
                workLogs = db.WorkLogs.Where(x => x.TaskId == taskId).OrderBy(x => x.Day);
            else
                workLogs = db.WorkLogs.Where(x => x.TaskId == taskId && x.UserId == currentUser.Id).OrderBy(x => x.Day);

            return map.Map<IEnumerable<WorkLogDto>>(workLogs);
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        public StoryTaskDto StartWork(Guid taskId)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .FirstOrDefault(x => x.Id == taskId);

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.Developer).Any())
                    throw new AppException("User must be Developer");

                if (task.UserId != currentUser.Id) throw new AppException("Only assigned user can start work progress");
            }

            if (task.UserId == null) throw new AppException("Task has no assigned user");

            if (task.Status != Data.Enums.UserStoryTaskStatus.Assigned)
                throw new AppException("Task must be assigned and inactive to start progress");

            if(db.Tasks.Where(x => x.Status == Data.Enums.UserStoryTaskStatus.InProgress && x.UserId == task.UserId).Any()) throw new AppException("Can only work on one task at a time");

            WorkLog previousDay = db.WorkLogs.Where(x => x.TaskId == taskId && x.UserId == task.UserId && x.Day < DateTime.Now.Date)
                .OrderBy(x => x.Day).FirstOrDefault();
            WorkLog workLog = db.WorkLogs.FirstOrDefault(x => x.TaskId == taskId && x.UserId == task.UserId && x.Day == DateTime.Now.Date);
            if (workLog == null)
            {
                workLog = new WorkLog
                {
                    TaskId = taskId,
                    UserId = task.UserId.Value,
                    Day = DateTime.Now,
                    HoursRemaining = (previousDay != null) ? previousDay.HoursRemaining : task.RemainingTime,
                    HoursWorked = 0
                };
                db.WorkLogs.Add(workLog);
            }

            task.ActiveFrom = DateTime.Now;
            task.Status = Data.Enums.UserStoryTaskStatus.InProgress;

            db.Tasks.Update(task);
            db.SaveChanges();

            return map.Map<StoryTaskDto>(task);
        }

        public StoryTaskDto EndWork(Guid taskId)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .FirstOrDefault(x => x.Id == taskId);

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.Developer).Any())
                    throw new AppException("User must be Developer");

                if (task.UserId != currentUser.Id) throw new AppException("Only assigned user can end work progress");
            }


            WorkLog previousDay = db.WorkLogs.Where(x => x.TaskId == taskId && x.UserId == task.UserId && x.Day < DateTime.Now.Date)
                .OrderBy(x => x.Day).FirstOrDefault();
            WorkLog workLog = db.WorkLogs.FirstOrDefault(x => x.TaskId == taskId && x.UserId == task.UserId && x.Day == DateTime.Now.Date);

            float hoursWorked = (float)(DateTime.Now - task.ActiveFrom).TotalHours;
            if (workLog != null)
            {
                
                workLog.HoursRemaining -= hoursWorked;
                workLog.HoursWorked += hoursWorked;
                if(workLog.HoursRemaining < 0)
                {
                    workLog.HoursRemaining = 0;
                }
                db.WorkLogs.Update(workLog);
            }
            else
            {
                workLog = new WorkLog
                {
                    TaskId = taskId,
                    UserId = task.UserId.Value,
                    Day = DateTime.Now,
                    HoursRemaining = (previousDay != null) ? previousDay.HoursRemaining - hoursWorked : task.RemainingTime - hoursWorked,
                    HoursWorked = hoursWorked
                };
                if (workLog.HoursRemaining < 0)
                {
                    workLog.HoursRemaining = 0;
                }
                db.WorkLogs.Add(workLog);
            }

            if(workLog.HoursRemaining == 0)
            {
                task.Status = Data.Enums.UserStoryTaskStatus.Finished;
            }
            else
            {
                task.Status = Data.Enums.UserStoryTaskStatus.Assigned;
            }
            
            task.RemainingTime = workLog.HoursRemaining;
            db.Tasks.Update(task);
            db.SaveChanges();

            return map.Map<StoryTaskDto>(task);
        }

        public IEnumerable<WorkLogDto> UpdateWorkLog(Guid logId, UpdateWorkLog model)
        {
            User currentUser = _authService.CurrentUser();

            WorkLog workLog = db.WorkLogs
                .Include(x => x.Task)
                    .ThenInclude(x => x.UserStory)
                .FirstOrDefault(x => x.Id == logId);
            if (workLog == null) throw new AppException("Could not find work log");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == workLog.Task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                //if (workLog.Task.UserId != currentUser.Id) throw new AppException("Only assigned user can update work log");
            }

            if (model.HoursWorked < 0) throw new AppException("Hours worked must be non-negative");
            if (model.HoursRemaining < 0) throw new AppException("Hours remaining must be non-negative");

            if (model.HoursWorked > 500) throw new AppException("Hours worked must be less than 500");
            if (model.HoursRemaining > 500) throw new AppException("Hours remaining must be less than 500");

            workLog.HoursWorked = model.HoursWorked;
            workLog.HoursRemaining = model.HoursRemaining;

            UserStoryTask task = db.Tasks.Find(workLog.Task.Id);
            if (workLog.Day == DateTime.Now.Date && task.Accepted && workLog.Task.UserId == currentUser.Id)
            {
                
                task.RemainingTime = workLog.HoursRemaining;
                if(task.RemainingTime == 0)
                {
                    task.Status = Data.Enums.UserStoryTaskStatus.Finished;
                } else
                {
                    task.Status = Data.Enums.UserStoryTaskStatus.Assigned;
                }
                db.Tasks.Update(task);
            }

            db.WorkLogs.Update(workLog);
            db.SaveChanges();

            return GetTaskWorkLogs(workLog.TaskId);
        }


        public IEnumerable<WorkLogDto> CreateWorkLog(Guid taskId, CreateWorkLog model)
        {
            User currentUser = _authService.CurrentUser();

            UserStoryTask task = db.Tasks
                .Include(x => x.UserStory)
                .FirstOrDefault(x => x.Id == taskId);
            if (task == null) throw new AppException("Could not find the task");

            if (currentUser.Role != Data.Enums.UserRole.Administrator)
            {
                UserProject up = db.UserProjects
                    .Include(x => x.ProjectRoles)
                    .FirstOrDefault(x => x.UserId == currentUser.Id && x.ProjectId == task.UserStory.ProjectId);
                if (up == null) throw new AppException("User must be part of the project");
                if (!up.Roles.Where(x => x == Data.Enums.ProjectRole.Developer).Any())
                    throw new AppException("User must be Developer");

                //if (task.UserId != model.UserId && currentUser.Id == model.UserId) 
                //    throw new AppException("Only assigned user can log work progress");
            }

            if (model.HoursWorked < 0) throw new AppException("Hours worked must be non-negative");
            if (model.HoursRemaining < 0) throw new AppException("Hours remaining must be non-negative");

            if (model.HoursWorked > 500) throw new AppException("Hours worked must be less than 500");
            if (model.HoursRemaining > 500) throw new AppException("Hours remaining must be less than 500");


            WorkLog workLog = db.WorkLogs.FirstOrDefault(x => x.TaskId == taskId && x.UserId == model.UserId && x.Day == model.Day.Date);
            if (workLog != null)
            {
                workLog.HoursWorked = model.HoursWorked;
                workLog.HoursRemaining = model.HoursRemaining;
                db.WorkLogs.Update(workLog);
            }
            else
            {
                workLog = new WorkLog
                {
                    HoursRemaining = model.HoursRemaining,
                    HoursWorked = model.HoursWorked,
                    Day = model.Day,
                    UserId = model.UserId,
                    TaskId = taskId
                };
                db.WorkLogs.Add(workLog);
            } 

            db.SaveChanges();

            return GetTaskWorkLogs(workLog.TaskId);
        }

    }
}
