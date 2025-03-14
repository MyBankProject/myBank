using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Data;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;

namespace MyBankWebApp.Repositories
{
    internal class AccountDetailsRepository(
        ApplicationDbContext context) : RepositoryBase<AccountDetail>(context), IAccountDetailsRepository
    {
        public async Task<bool> AnyByIdAsync(int id) => await context.AccountDetails.AnyAsync(account => account.UserId == id);

        public async Task<AccountDetail> GetAccountByIbanAsync(string iban) =>
                    await context.AccountDetails.FirstOrDefaultAsync(account => account.IBAN == iban)
                    ?? throw new UserNotFoundException("Reciver not found");

        //TODO: przenieść do klasy bazowej po poprawieniu ID w całej bazie danych
        public async Task<AccountDetail> GetByIdAsync(int id, Func<IQueryable<AccountDetail>, IQueryable<AccountDetail>>? inclue = null)
        {
            IQueryable<AccountDetail> query = context.AccountDetails;
            if (inclue != null)
            {
                query = inclue(query);
            }

            return await query.FirstOrDefaultAsync(account => account.UserId == id)
                ?? throw new UserNotFoundException("Sender not found");
        }
    }
}