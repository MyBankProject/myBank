using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Data;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;

namespace MyBankWebApp.Repositories
{
    public class AccountDetailsRepository(ApplicationDbContext context) :
        RepositoryBase<Account>(context), IAccountDetailsRepository
    {
        public async Task<bool> AnyByIdAsync(int id) => await context.Accounts.AnyAsync(account => account.Id == id);

        public async Task<Account?> GetAccountByIbanAsync(string iban) =>
                    await context.Accounts.FirstOrDefaultAsync(account => account.IBAN == iban);

        //TODO: przenieść do klasy bazowej po poprawieniu ID w całej bazie danych
        public async Task<Account?> GetByIdAsync(int id, Func<IQueryable<Account>, IQueryable<Account>>? inclue = null)
        {
            IQueryable<Account> query = context.Accounts;
            if (inclue != null)
            {
                query = inclue(query);
            }
            return await query.FirstOrDefaultAsync(account => account.Id == id);
        }
    }
}