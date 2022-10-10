using System;
using smrpo_be.Data.WebModels;
using System.Collections.Generic;
using smrpo_be.Services;
using smrpo_be.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smrpo_be.Data.Requests.UserStory;
using smrpo_be.Data.Requests;

namespace FileTool.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] TaskCreate model)
        {
            try
            {
                IEnumerable<StoryTaskDto> tasks = _taskService.CreateStoryTask(model);
                return Ok(tasks);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/assign/{userId}")]
        public IActionResult AssignTask(Guid id, Guid userId)
        {
            try
            {
                StoryTaskDto task = _taskService.AssignTask(id, userId);
                return Ok(task);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/accept")]
        public IActionResult AcceptTask(Guid id)
        {
            try
            {
                StoryTaskDto task = _taskService.AcceptTask(id);
                return Ok(task);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/decline")]
        public IActionResult DeclineTask(Guid id)
        {
            try
            {
                StoryTaskDto task = _taskService.DeclineTask(id);
                return Ok(task);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/finish")]
        public IActionResult FinishTask(Guid id)
        {
            try
            {
                StoryTaskDto task = _taskService.FinishTask(id);
                return Ok(task);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult EditTask(Guid id,[FromBody] TaskEdit model)
        {
            try
            {
                IEnumerable<StoryTaskDto> tasks = _taskService.EditTask(id, model);
                return Ok(tasks);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(Guid id)
        {
            try
            {
                IEnumerable<StoryTaskDto> tasks = _taskService.DeleteTask(id);
                return Ok(tasks);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{id}/work_logs")]
        public IActionResult WorkLogs(Guid id)
        {
            try
            {
                IEnumerable<WorkLogDto> workLogs = _taskService.GetTaskWorkLogs(id);
                return Ok(workLogs);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/start")]
        public IActionResult StartWork(Guid id)
        {
            try
            {
                StoryTaskDto task = _taskService.StartWork(id);
                return Ok(task);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/end")]
        public IActionResult EndWork(Guid id)
        {
            try
            {
                StoryTaskDto task = _taskService.EndWork(id);
                return Ok(task);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/work_logs/{workLogId}")]
        public IActionResult UpdateWorkLog(Guid id, Guid workLogId, [FromBody] UpdateWorkLog model)
        {
            try
            {
                IEnumerable<WorkLogDto> workLogs = _taskService.UpdateWorkLog(workLogId, model);
                return Ok(workLogs);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/work_logs")]
        public IActionResult CreateWorkLog(Guid id,[FromBody] CreateWorkLog model)
        {
            try
            {
                IEnumerable<WorkLogDto> workLogs = _taskService.CreateWorkLog(id, model);
                return Ok(workLogs);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
