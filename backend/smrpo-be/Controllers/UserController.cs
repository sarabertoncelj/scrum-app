using System;
using System.Collections.Generic;
using smrpo_be.Data.WebModels;
using smrpo_be.Services;
using smrpo_be.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using smrpo_be.Data.Requests.User;

namespace FileTool.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserAuthentication model)
        {
            UserDto user = _userService.Authenticate(model);
            if (user == null) return BadRequest(new { message = "Username or password is incorect" });

            return Ok(user);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody]UserRegistration model)
        {
            try
            {
                UserDto user = _userService.Create(model);
                return Ok(user);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<UserDto> users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                UserDto user = _userService.Get(id);
                return Ok(user);
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public IActionResult Update([FromBody]UserUpdate model)
        {
            try
            {
                _userService.Update(model);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery(Name = "username")] string username)
        {
            IEnumerable<UserSearchableDto> users =  _userService.Search(username);
            return Ok(users);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            _userService.Delete(id);
            return Ok();
        }

    }
}
