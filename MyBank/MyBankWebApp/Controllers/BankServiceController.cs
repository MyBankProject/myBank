using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Data;
using MyBankWebApp.DTOs;
using MyBankWebApp.Models;
using MyBankWebApp.Services.Transactions.Abstractions;
using System.Diagnostics;

namespace MyBankWebApp.Controllers
{
    public class BankServiceController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly ILogger<BankServiceController> logger;
        private readonly IMapper mapper;
        private readonly ITransactionService transactionService;

        public BankServiceController(
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<BankServiceController> logger,
            ITransactionService transactionService)
        {
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
            this.transactionService = transactionService;
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
        public async Task<IActionResult> Transaction(NewTransactionDto newTransactionDto)
        {
            if (newTransactionDto != null && newTransactionDto.Amount > 0)
            {
                //TODO: Odkomentować po zrobieniu logowania (Id użytkownika, który wysyła przelew jest dla bezpieczeństwa pobierany dopiero tutaj, żeby nie można było wpisać tego
                //z inspektora w przeglądarce i robić za kogoś przelewów z jego konta XD
                //var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //if (!int.TryParse(userIdClaim, out int userId))
                //{
                //    return Unauthorized();
                //}
                //newTransactionDto.SenderId = userId;
                newTransactionDto.SenderId = 5;

                try
                {
                    await transactionService.AddTransactionAsync(newTransactionDto);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unable to create transaction: {Transaction}.", newTransactionDto);
                    TempData["ErrorMessage"] = $"Transaction Failed: {ex.Message}";
                    return RedirectToAction(nameof(Index));
                }
                TempData["SuccessMessage"] = "Transaction Successful!";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Transaction Failed";
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