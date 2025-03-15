using MyBankWebApp.Models.Abstractions;

namespace MyBankWebApp.Repositories.Abstractions
{
    public interface IAccountDetailsRepository : IRepositoryBase<IAccountDetail>
    {
        Task<bool> AnyByIdAsync(int id);

        Task<IAccountDetail> GetAccountByIbanAsync(string iban);

        Task<IAccountDetail> GetByIdAsync(int id, Func<IQueryable<IAccountDetail>, IQueryable<IAccountDetail>>? inclue = null);
    }
}