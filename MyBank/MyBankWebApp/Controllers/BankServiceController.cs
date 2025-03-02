using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Data;
using MyBankWebApp.DTOs;
using MyBankWebApp.Mappers;
using MyBankWebApp.Models;
using System.Diagnostics;

namespace MyBankWebApp.Controllers
{
    public class BankServiceController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BankServiceController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       // [Authorize]
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
                var AccountDto = GetAccountDetailDto(user);
                return View(AccountDto);
            }
            return Error();
        }

        private AccountDetailDto GetAccountDetailDto(AccountDetail? user)
        {
            if (user != null)
            {
                return mapper.Map<AccountDetailDto>(user);
            }
            throw new ArgumentNullException(nameof(user), "User could not be found.");
        }
    }
}