using System;
using System.Collections.Generic;
using smrpo_be.Data.WebModels;
using smrpo_be.Services;
using smrpo_be.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smrpo_be.Data.Requests.User;
using smrpo_be.Data.Requests.Project;
using smrpo_be.Data.Enums;

namespace FileTool.Controllers
{
    [Authorize]
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IUserStoryService _userStoryService;
        private readonly ISprintService _sprintService;

        public ProjectController(IProjectService projectService, IUserStoryService userStoryService, ISprintService sprintService)
        {
            _projectService = projectService;
            _userStoryService = userStoryService;
            _sprintService = sprintService;
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                IEnumerable<ProjectDto> projects = _projectService.GetAll();
                return Ok(projects);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            try { 
                ProjectDto project = _projectService.Get(id);
                return Ok(project);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody]ProjectCreate model)
        {
            try
            {
                ProjectDto project = _projectService.Create(model);
                return Ok(project);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Edit(Guid id, [FromBody] ProjectEdit model)
        {
            try
            {
                ProjectDto project = _projectService.EditProject(id, model);
                return Ok(project);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{id}/users")]
        public IActionResult GetUsers(Guid id)
        {
            try
            {
                IEnumerable<ProjectUserDto> users = _projectService.GetProjectUsers(id);
                return Ok(users);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/posts")]
        public IActionResult GetPosts(Guid id)
        {
            try
            {
                IEnumerable<ProjectPostDto> projectPosts = _projectService.GetProjectPosts(id);
                return Ok(projectPosts);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/posts")]
        public IActionResult AddPost(Guid id, [FromBody]ProjectAddPost model)
        {
            try
            {
                IEnumerable<ProjectPostDto> posts = _projectService.AddPost(id, model);
                return Ok(posts);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/users")]
        public IActionResult AddUser(Guid id, [FromBody]ProjectAddUser model)
        {
            try
            {
                IEnumerable<ProjectUserDto> users = _projectService.AddUser(id, model);
                return Ok(users);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/users/{userId}")]
        public IActionResult RemoveUser(Guid id, Guid userId)
        {
            try
            {
                IEnumerable<ProjectUserDto> users = _projectService.RemoveUser(id, userId);
                return Ok(users);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/users/{userId}/{role}")]
        public IActionResult RemoveUserRole(Guid id, Guid userId, ProjectRole role)
        {
            try
            {
                IEnumerable<ProjectUserDto> users = _projectService.RemoveRole(id, userId, role);
                return Ok(users);
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
                _projectService.Delete(id);
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
                IEnumerable<UserStoryDto> userStories = _userStoryService.GetProjectUserStories(id);
                return Ok(userStories);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/sprints")]
        public IActionResult GetSprints(Guid id)
        {
            try
            {
                IEnumerable<SprintDto> sprints = _sprintService.GetProjectSprints(id);
                return Ok(sprints);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
