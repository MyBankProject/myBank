using MyBankWebApp.Data;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;

namespace MyBankWebApp.Repositories
{
    public class AccountDetailsRepository : IAccountDetailsRepository
    {
        //TODO: Wydłubać klasę bazową dla klas Repository
        private readonly ApplicationDbContext context;

        public AccountDetailsRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(AccountDetail transaction) => context.AccountDetails.Add(transaction);

        public bool AnyById(int id) => context.AccountDetails.Any(account => account.UserId == id);

        public AccountDetail GetAccountByIban(string iban) =>
                    context.AccountDetails.FirstOrDefault(account => account.IBAN == iban)
            ?? throw new UserNotFoundException("Reciver not found");

        public AccountDetail GetAccountById(int id, Func<IQueryable<AccountDetail>, IQueryable<AccountDetail>>? inclue = null)
        {
            IQueryable<AccountDetail> query = context.AccountDetails;
            if (inclue != null)
            {
                query = inclue(query);
            }

            return query.FirstOrDefault(account => account.UserId == id)
                ?? throw new UserNotFoundException("Sender not found");
        }

        public void SaveChanges() => context.SaveChanges();
    }
}