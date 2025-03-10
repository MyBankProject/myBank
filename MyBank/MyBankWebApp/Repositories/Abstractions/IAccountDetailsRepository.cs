using MyBankWebApp.Models;

namespace MyBankWebApp.Repositories.Abstractions
{
    public interface IAccountDetailsRepository
    {
        void Add(AccountDetail transaction);
        bool AnyById(int id);
        AccountDetail GetAccountByIban(string iban);
        AccountDetail GetAccountById(int id, Func<IQueryable<AccountDetail>, IQueryable<AccountDetail>>? inclue = null);
        void SaveChanges();
    }
}