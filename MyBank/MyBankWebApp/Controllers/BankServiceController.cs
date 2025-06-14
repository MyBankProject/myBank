using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models.Users;
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
        private readonly IAccountRepository accountRepository = accountRepository;
        private readonly IAccountService accountService = accountService;
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
                User user = await userService.GetUserByStringIdAsync(idString);
                AccountViewModel accountVm = user.AccountId != null
                    ? await accountService.GetAccountVmByIdAsync((int)user.AccountId)
                    : throw new AccountNotFountException($"Could not fount account for User {user.Email}");
                return View(accountVm);
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
            if (newTransactionDto.Amount > 0)
            {
                try
                {
                    string? stringId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    User senderAccount = await userService.GetUserByStringIdAsync(stringId);
                    newTransactionDto.SenderId = senderAccount.AccountId
                        ?? throw new AccountNotFountException($"Could not find account for user {senderAccount.Email}");
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

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositViewModel depositViewModel)
        {
            if (depositViewModel.Amount > 0)
            {
                try
                {
                    string? stringId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    User reciverAccount = await userService.GetUserByStringIdAsync(stringId, q => q.Include(u => u.Account));
                    depositViewModel.ReceiverIBAN = reciverAccount?.Account?.IBAN;
                    await transactionService.AddDepositAsync(depositViewModel);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unable to create deposit: {deposit}.", depositViewModel);
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