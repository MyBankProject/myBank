using Microsoft.AspNetCore.Mvc;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Services.Abstractions;

namespace MyBankWebApp.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            userService.RegisterUser(dto);
            return Ok();
        }
    }
}
