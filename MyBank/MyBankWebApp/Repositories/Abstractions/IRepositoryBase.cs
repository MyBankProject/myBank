using Microsoft.EntityFrameworkCore.Storage;
using MyBankWebApp.Models;

namespace MyBankWebApp.Repositories.Abstractions
{
    public interface IRepositoryBase<T> where T : class
    {
        Task AddAsync(T transaction);

        Task<IDbContextTransaction> BeginTransactionAsync();

        Task<Account?> GetByIdAsync(int id, Func<IQueryable<Account>, IQueryable<Account>>? inclue = null);

        Task SaveAsync();
    }
}