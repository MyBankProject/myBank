using Microsoft.EntityFrameworkCore.Storage;

namespace MyBankWebApp.Repositories.Abstractions
{
    public interface IRepositoryBase<T> where T : class
    {
        Task AddAsync(T transaction);

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task SaveAsync();
    }
}