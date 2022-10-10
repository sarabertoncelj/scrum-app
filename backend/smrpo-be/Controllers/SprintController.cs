using smrpo_be.Data.WebModels;
using smrpo_be.Services;
using smrpo_be.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smrpo_be.Data.Requests.Sprint;
using System;
using System.Collections.Generic;

namespace FileTool.Controllers
{
    [Authorize]
    [Route("api/sprints")]
    [ApiController]
    public class SprintController : ControllerBase
    {
        private readonly ISprintService _sprintService;

        public SprintController(ISprintService sprintService)
        {
            _sprintService = sprintService;
        }

        [HttpPost]
        public IActionResult Create([FromBody]SprintCreate model)
        {
            try
            {
                SprintDto sprint = _sprintService.Create(model);
                return Ok(sprint);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(Guid id, [FromBody]SprintUpdate model)
        {
            try
            {
                SprintDto sprint = _sprintService.Update(id, model);
                return Ok(sprint);
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
                _sprintService.Delete(id);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/user_stories")]
        public IActionResult GetUserStories(Guid id)
        {
            try
            {
                IEnumerable<UserStoryDto> userStories = _sprintService.GetUserStories(id);
                return Ok(userStories);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/user_stories/{userStoryId}")]
        public IActionResult AddUserStory(Guid id, Guid userStoryId)
        {
            try
            {
                IEnumerable<UserStoryDto> userStories = _sprintService.AddUserStory(id, userStoryId);
                return Ok(userStories);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
