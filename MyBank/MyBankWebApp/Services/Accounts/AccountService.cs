using AutoMapper;
using MyBankWebApp.Models;
using MyBankWebApp.Services.Accounts.Abstractions;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Services.Accounts
{
    public class AccountService(IMapper mapper) : IAccountService
    {
        private readonly IMapper mapper = mapper;

        public AccountViewModel GetAccountVM(Account user)
        {
            AccountViewModel accountVM = mapper.Map<AccountViewModel>(user);
            accountVM.Transactions = accountVM.Transactions?.OrderByDescending(x => x.CreationTime).ToList();
            return accountVM;
        }
    }
}