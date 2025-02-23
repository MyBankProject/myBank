using Microsoft.AspNetCore.Mvc;
using MyBankWebApp.Data;
using MyBankWebApp.Models;
using System.Diagnostics;

namespace MyBankWebApp.Controllers
{
    public class BankServiceController : Controller
    {
        private readonly ApplicationDbContext context;

        public BankServiceController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(int id)
        {
            if (ModelState.IsValid)
            {
                //TODO: Usunąć przypisanie Id, kiedy już będzie logowanie na konto
                id = 5;
                AccountDetail? user = context.AccountDetails.FirstOrDefault(x => x.UserId == id);
                return View(user);
            }
            return Error();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
