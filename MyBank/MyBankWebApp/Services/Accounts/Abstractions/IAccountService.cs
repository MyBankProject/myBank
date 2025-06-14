using MyBankWebApp.Models;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Services.Accounts.Abstractions
{
    public interface IAccountService
    {
        Task<Account> CreateAccount(string countryCode);

        Task<AccountViewModel> GetAccountViewModelByIdAsync(int id);
    }
}