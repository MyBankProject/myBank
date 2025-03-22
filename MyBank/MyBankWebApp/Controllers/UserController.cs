using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Models;
using MyBankWebApp.Services;
using MyBankWebApp.Services.UserServices.Abstractions;

namespace MyBankWebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Login()
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
            return RedirectToAction("SuccessRegister");
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
            //return RedirectToAction("SuccessLogin"); // trzeba bedzie gdzies przekierowac zamiast wyswietlac tokena xD
            return Content(token);
        }

       // [HttpPost] bedzie potrzebne zeby wylogowac sie poprzez klikniecie guzika.. poki co dziala przez link
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
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
