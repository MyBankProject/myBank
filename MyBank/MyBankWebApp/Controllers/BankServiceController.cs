using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBankWebApp.DTOs;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Transactions.Abstractions;
using System.Diagnostics;

namespace MyBankWebApp.Controllers
{
    public class BankServiceController(
        IAccountDetailsRepository accountDetailsRepository,
        IMapper mapper,
        ILogger<BankServiceController> logger,
        ITransactionService transactionService) : Controller
    {
        private readonly IAccountDetailsRepository accountDetailsRepository = accountDetailsRepository;
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
            if (ModelState.IsValid)
            {
                //TODO: Usunąć przypisanie Id, kiedy już będzie logowanie na konto
                id = 5;
                AccountDetail user = await accountDetailsRepository
                    .GetByIdAsync(id, query => query.Include(a => a.RecivedTransactions)
                    .Include(a => a.SentTransactions));
                AccountDetailDto AccountDto = GetAccountDetailDto(user);
                return View(AccountDto);
            }
            return Error();
        }

        public async Task<IActionResult> Transaction(int id)
        {
            if (await accountDetailsRepository.AnyByIdAsync(id))
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