using MyBankWebApp.Models;

namespace MyBankWebApp.Repositories.Abstractions
{
    public interface IAccountDetailsRepository : IRepositoryBase<Account>
    {
        Task<bool> AnyByIdAsync(int id);

        Task<Account?> GetAccountByIbanAsync(string iban);

        Task<Account?> GetByIdAsync(int id, Func<IQueryable<Account>, IQueryable<Account>>? inclue = null);
    }
}