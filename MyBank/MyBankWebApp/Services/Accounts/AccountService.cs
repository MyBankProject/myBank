using AutoMapper;
using IbanNet.Registry;
using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Accounts.Abstractions;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Services.Accounts
{
    public class AccountService(IMapper mapper, IAccountRepository accountRepository) : IAccountService
    {
        private readonly IMapper mapper = mapper;
        private readonly IAccountRepository accountRepository = accountRepository;
        private readonly IbanGenerator ibanGenerator = new IbanGenerator();

        public async Task<Account> CreateAccount(string countryCode)
        {
            var account = new Account()
            {
                CountryCode = countryCode,
                IBAN = ibanGenerator.Generate(countryCode).ToString(),
                Balance = 0
            };
            await accountRepository.AddAsync(account);
            await accountRepository.SaveAsync();
            return account;
        }

        public async Task<AccountViewModel> GetAccountVmAsync(int id)
        {
            Account? user = await accountRepository.GetByIdAsync(id, query => query
                    .Include(a => a.ReceivedTransactions)
                    .Include(a => a.SentTransactions)) ?? throw new UserNotFoundException($"Could not find user {id}");
            AccountViewModel accountVM = mapper.Map<AccountViewModel>(user);
            if (accountVM.Transactions != null)
            {
                foreach (TransactionViewModel t in accountVM.Transactions)
                {
                    t.OtherSideOfTransaction = mapper.Map<AccountViewModel>(await accountRepository.GetByIdAsync(t.OtherSideOfTransactionId));
                }
                accountVM.Transactions = accountVM.Transactions.OrderByDescending(x => x.CreationTime).ToList();
            }
            return accountVM;
        }
    }
}