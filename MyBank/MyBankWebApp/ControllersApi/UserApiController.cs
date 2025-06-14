using Microsoft.AspNetCore.Mvc;
using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Services.UserServices.Abstractions;

namespace MyBankWebApp.ControllersApi
{
    [ApiController]
    [Route("api/user")]
    public class UserApiController : ControllerBase
    {
        private readonly IUserService userService;

        public UserApiController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto dto)
        {
            string token = userService.GenerateJwt(dto);
            return Ok(token);
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            userService.RegisterUser(dto);
            return Ok();
        }
    }
}