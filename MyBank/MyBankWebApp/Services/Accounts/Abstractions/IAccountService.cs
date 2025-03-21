using MyBankWebApp.Models;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Services.Accounts.Abstractions
{
    public interface IAccountService
    {
        Task<AccountViewModel> GetAccountVmAsync(int id);
    }
}