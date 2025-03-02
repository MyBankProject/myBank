using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Models;
using MyBankWebApp.Services;
using MyBankWebApp.Services.Abstractions;

namespace MyBankWebApp.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserService userService;

        public RegisterController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(UserViewModel model)
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
                return View("Index", model);
            }
            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return Content("User registered successfully!");
        }
    }
}
