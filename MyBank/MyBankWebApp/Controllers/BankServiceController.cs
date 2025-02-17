using Microsoft.AspNetCore.Mvc;

namespace MyBankWebApp.Controllers
{
    public class BankServiceController : Controller
    {
        public BankServiceController()
        {
            //TODO: Dodać w konstruktorze model jednego użytkownika - na razie trzeba go najpierw stworzyć
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
