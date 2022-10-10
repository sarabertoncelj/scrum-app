using System;
using smrpo_be.Data.WebModels;
using System.Collections.Generic;
using smrpo_be.Services;
using smrpo_be.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smrpo_be.Data.Requests.UserStory;

namespace FileTool.Controllers
{
    [Authorize]
    [Route("api/user_stories")]
    [ApiController]
    public class UserStoryController : ControllerBase
    {
        private readonly IUserStoryService _userStoryService;
        private readonly ITaskService _taskService;

        public UserStoryController(IUserStoryService userStoryService, ITaskService taskService)
        {
            _userStoryService = userStoryService;
            _taskService = taskService;
        }

        [HttpPost]
        public IActionResult Create([FromBody]UserStoryCreate model)
        {
            try
            {
                UserStoryDto userStoryDto = _userStoryService.Create(model);
                return Ok(userStoryDto);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _userStoryService.Delete(id);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]UserStoryUpdate model)
        {
            try
            {
                UserStoryDto userStoryDto = _userStoryService.Update(id, model);
                return Ok(userStoryDto);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/user_story_times")]
        public IActionResult GetUserStoryTimes(Guid id)
        {
            try
            {
                IEnumerable<UserStoryTimeDto> userStoryTimesDto = _userStoryService.GetUserStoryTimes(id);
                return Ok(userStoryTimesDto);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/user_story_times")]
        public IActionResult UpdateUserStoryTime(Guid id, [FromBody]UserStoryTimeUpdate model)
        {
            try
            {
                UserStoryTimeDto userStoryTimeDto = _userStoryService.UpdateUserStoryTime(id, model);
                return Ok(userStoryTimeDto);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/tasks")]
        public IActionResult GetStoryTasks(Guid id)
        {
            try
            {
                IEnumerable<StoryTaskDto> tasks = _taskService.GetStoryTasks(id);
                return Ok(tasks);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/accept")]
        public  IActionResult ApproveUserStory(Guid id)
        {
            try
            {
                UserStoryDto userStory = _userStoryService.Accept(id);
                return Ok(userStory);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/reject")]
        public IActionResult DeclineUserStory(Guid id, [FromBody] UserStoryDecline model)
        {
            try
            {
                UserStoryDto userStory = _userStoryService.Reject(id, model);
                return Ok(userStory);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
