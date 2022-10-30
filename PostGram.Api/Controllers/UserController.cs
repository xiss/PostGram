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
        public async Task CreateUser(CreateUserModel model)
        {
            await  _userService.CreateUser(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<List<UserModel>> GetUsers()
        {
            return await _userService.GetUsers();
        }

        [HttpGet]
        [Authorize]
        public async Task<UserModel> GetCurrentUser()
        {
            string? userIdStr = User.Claims.FirstOrDefault(c => c.Type == nameof(DAL.Entities.User.Id))?.Value;
            if (Guid.TryParse(userIdStr, out var userId))
                return await _userService.GetUser(userId);
            //TODO Разве может выбросить это исключенние, ведь указан атрибут Authorize
            throw new AuthorizationException(message: "You are not authorized");
        }
    }

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<TokenModel> Token(TokenRequestModel model)
        {
            return await _userService.GetToken(model.Login,model.Password);
        }

    }
}
