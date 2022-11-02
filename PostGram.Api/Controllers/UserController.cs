using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostGram.Api.Models;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            try
            {
                await _userService.CreateUser(model);
            }
            catch (DBCreatePostGramException e)
            {
               return StatusCode(500, e.Message);
            }
            
            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            List<UserModel> users = new();
            try
            {
                users = await _userService.GetUsers();
            }
            catch (AuthorizationPostGramException e)
            {
                return Forbid(e.Message);
            }

            return Ok(users);
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserModel>> GetCurrentUser()
        {
            try
            {
                string? userIdStr = User.Claims.FirstOrDefault(c => c.Type == nameof(DAL.Entities.User.Id))?.Value;
                if (Guid.TryParse(userIdStr, out var userId))
                    return await _userService.GetUser(userId);
                throw new AuthorizationPostGramException("You are not authorized");
            }
            catch (UserNotFoundPostGramException e)
            {
                return NotFound(e.Message);
            }
            catch (AuthorizationPostGramException e)
            {
                return Unauthorized(e.Message);
            }
        }
    }
}
