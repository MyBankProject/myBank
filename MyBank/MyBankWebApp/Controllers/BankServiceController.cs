using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index(int id)
        {
            if (ModelState.IsValid)
            {
                //TODO: Usunąć przypisanie Id, kiedy już będzie logowanie na konto
                id = 5;
                AccountDetail? user = context.AccountDetails
                    .Include(a => a.RecivedTransactions)
                    .Include(a => a.SentTransactions)
                    .FirstOrDefault(x => x.UserId == id);
                ReadTransactions(user);
                return View(user);
            }
            return Error();
        }

        private static void ReadTransactions(AccountDetail? user)
        {
            //TODO: ogarnąć null-menage
            if (user != null)
            {
                List<Transaction> recived = user.RecivedTransactions?.ToList() ?? [];
                List<Transaction> send = user.SentTransactions?.ToList() ?? [];
                user.Transactions = recived.Concat(send)
                    .OrderBy(t => t.CreationTime)
                    .ToList();
            }
        }
    }
}