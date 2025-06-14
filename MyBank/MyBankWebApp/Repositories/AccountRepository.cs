using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Data;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;

namespace MyBankWebApp.Repositories
{
    public class AccountRepository(ApplicationDbContext context) : RepositoryBase<Account>(context), IAccountRepository
    {
        public async Task<bool> AnyByIdAsync(int id) => await context.Accounts.AnyAsync(account => account.Id == id);

        public async Task<Account?> GetAccountByIbanAsync(string iban) => 
            await context.Accounts.FirstOrDefaultAsync(account => account.IBAN == iban);
    }
}