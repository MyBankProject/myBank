using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Transactions.Abstractions;
using MyBankWebApp.ViewModels;
using System.Diagnostics;

namespace MyBankWebApp.Controllers
{
    public class BankServiceController(
        IAccountRepository accountDetailsRepository,
        IMapper mapper,
        ILogger<BankServiceController> logger,
        ITransactionService transactionService) : Controller
    {
        private readonly IAccountRepository accountDetailsRepository = accountDetailsRepository;
        private readonly ILogger<BankServiceController> logger = logger;
        private readonly IMapper mapper = mapper;
        private readonly ITransactionService transactionService = transactionService;

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
            var user = await accountDetailsRepository
                .GetByIdAsync(id, query => query.Include(a => a.ReceivedTransactions)
                .Include(a => a.SentTransactions));

            if (user == null)
            {
                return Error();
            }

            AccountViewModel accountDto = GetAccountDto(user);
            return View(accountDto);
        }

        public async Task<IActionResult> NewTransaction(int id)
        {
            if (await accountDetailsRepository.AnyByIdAsync(id))
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

        private AccountViewModel GetAccountDto(Account? user)
        {
            if (user != null)
            {
                return mapper.Map<AccountViewModel>(user);
            }
            throw new ArgumentNullException(nameof(user), "User could not be found.");
        }
    }
}