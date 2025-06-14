using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Services.UserServices.Abstractions;
using MyBankWebApp.ViewModels;
using Newtonsoft.Json;

namespace MyBankWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> logger;
        private readonly IMapper mapper;
        private readonly IUserService userService;

        public UserController(IUserService userService, ILogger<UserController> logger, IMapper mapper)
        {
            this.userService = userService;
            this.logger = logger;
            this.mapper = mapper;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsEmailTaken()
        {
            using var reader = new System.IO.StreamReader(Request.Body);
            string body = await reader.ReadToEndAsync();
            Dictionary<string, string>? emailData = JsonConvert.DeserializeObject<Dictionary<string, string>>(body);
            if (emailData == null || !emailData.TryGetValue("email", out string? email))
            {
                return BadRequest("Invalid request body or missing 'email' key.");
            }

            bool isEmailTaken = await userService.AnyUserByQueryAsync(query => query.Where(u => u.Email == email));
            return Json(isEmailTaken);
        }

        [HttpGet]
        public IActionResult Login(string? error = null)
        {
            ViewBag.LoginError = error;
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDto dto)
        {
            string token = userService.GenerateJwt(dto);

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddHours(1)
            });
            return RedirectToAction("Index", "BankService");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                RegisterUserDto dto = mapper.Map<RegisterUserDto>(model);
                await userService.RegisterUser(dto);
                TempData["SuccessMessage"] = "Registration Successful!";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to register: {UserViewModel}", model);
                TempData["ErrorMessage"] = $"Register Failed: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult SuccessLogin()
        {
            return Content("User login successfully!");
        }

        public IActionResult SuccessRegister()
        {
            return Content("User registered successfully!");
        }
    }
}