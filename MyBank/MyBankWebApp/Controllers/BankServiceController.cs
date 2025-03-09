using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Data;
using MyBankWebApp.DTOs;
using MyBankWebApp.Models;
using System.Diagnostics;
using System.Security.Claims;

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

        public IActionResult Transaction(int id)
        {
            if (context.AccountDetails.Any(user => user.UserId == id))
            {
                return View(new NewTransactionDto() { SenderId = id });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Transaction(NewTransactionDto newTransactionDto)
        {
            //TODO: zaimplementować użycie serwisu tutaj
            if (newTransactionDto != null && newTransactionDto.Amount > 0)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }
                newTransactionDto.SenderId = userId;
            }

            return RedirectToAction(nameof(Index));
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