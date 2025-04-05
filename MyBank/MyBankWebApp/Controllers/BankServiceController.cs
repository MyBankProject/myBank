using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBankWebApp.Entities;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Accounts.Abstractions;
using MyBankWebApp.Services.Transactions.Abstractions;
using MyBankWebApp.Services.UserServices.Abstractions;
using MyBankWebApp.ViewModels;
using System.Diagnostics;
using System.Security.Claims;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Controllers
{
    [Authorize(Roles = nameof(Roles.User))]
    public class BankServiceController(
        ILogger<BankServiceController> logger,
        ITransactionService transactionService,
        IUserService userService,
        IAccountService accountService,
        IAccountRepository accountRepository) : Controller
    {
        private readonly IAccountService accountService = accountService;
        private readonly IAccountRepository accountRepository = accountRepository;
        private readonly ILogger<BankServiceController> logger = logger;
        private readonly ITransactionService transactionService = transactionService;
        private readonly IUserService userService = userService;

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> Index()
        {
            if (!ModelState.IsValid)
            {
                return Error();
            }
            try
            {
                string? idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (int.TryParse(idString, out int id))
                {
                    User user = await userService.GetUserAsync(id);
                    if (user.AccountId != null)
                    {
                        int accountId = (int)user.AccountId;
                        AccountViewModel account = await accountService.GetAccountVmAsync(accountId);
                        return View(account);
                    }
                    throw new AccountNotFountException($"Could not find account asigned to user. accountId: {user.AccountId}, user: {user.Email}");
                }
                throw new InvalidIdException("Could not get user Id");
            }
            catch (InvalidIdException ex)
            {
                logger.LogError(ex, ex.Message);
                return Error();
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