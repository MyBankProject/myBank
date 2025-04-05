using Microsoft.AspNetCore.Mvc;
using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Services.UserServices.Abstractions;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
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
                HttpOnly = true, // Zabezpiecza przed dostępem przez JavaScript
                Secure = true, // Włączone dla HTTPS
                Expires = DateTime.UtcNow.AddHours(1) // Token ważny przez 1 godzinę
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
        public IActionResult Register(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dto = new RegisterUserDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Nationality = model.Nationality,
                DateOfBirth = model.DateOfBirth
            };

            var errors = userService.RegisterUser(dto);
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
                return View("Register", model);
            }
            return RedirectToAction("Index", "Home");
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