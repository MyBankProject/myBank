using MyBankWebApp.Models;

namespace MyBankWebApp.Repositories.Abstractions
{
    public interface IAccountDetailsRepository : IRepositoryBase<AccountDetail>
    {
        Task<bool> AnyByIdAsync(int id);

        Task<AccountDetail?> GetAccountByIbanAsync(string iban);

        Task<AccountDetail?> GetByIdAsync(int id, Func<IQueryable<AccountDetail>, IQueryable<AccountDetail>>? inclue = null);
    }
}