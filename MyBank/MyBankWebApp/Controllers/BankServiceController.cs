using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Accounts.Abstractions;
using MyBankWebApp.Services.Transactions.Abstractions;
using MyBankWebApp.ViewModels;
using System.Diagnostics;

namespace MyBankWebApp.Controllers
{
    public class BankServiceController(
        IAccountRepository accountRepository,
        ILogger<BankServiceController> logger,
        ITransactionService transactionService,
        IAccountService accountService) : Controller
    {
        private readonly IAccountRepository accountRepository = accountRepository;
        private readonly ILogger<BankServiceController> logger = logger;
        private readonly ITransactionService transactionService = transactionService;
        private readonly IAccountService accountService = accountService;

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index(int id)
        {
            if (!ModelState.IsValid)
            {
                return Error();
            }
            // TODO: Usunąć przypisanie Id, kiedy już będzie logowanie na konto
            id = 5;
            try
            {
                Task<AccountViewModel> accountVM = accountService.GetAccountVmAsync(id);
                return View(await accountVM);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return Error();
            }
        }

        public async Task<IActionResult> NewTransaction(int id)
        {
            if (await accountRepository.AnyByIdAsync(id))
            {
                return View(new NewTransactionViewModel() { SenderId = id });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> NewTransaction(NewTransactionViewModel newTransactionDto)
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
    }
}