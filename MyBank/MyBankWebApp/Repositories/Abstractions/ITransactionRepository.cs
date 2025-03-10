using Microsoft.EntityFrameworkCore.Storage;
using MyBankWebApp.Models;

namespace MyBankWebApp.Repositories.Abstractions
{
    public interface ITransactionRepository
    {
        void AddTransaction(Transaction transaction);
        Task<IDbContextTransaction> BeginTransactionAsync();
        void Save();
    }
}