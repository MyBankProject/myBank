using Microsoft.EntityFrameworkCore.Storage;
using MyBankWebApp.Data;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;

namespace MyBankWebApp.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext context;

        public TransactionRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void AddTransaction(Transaction transaction) => context.Transactions.Add(transaction);

        public void Save() => context.SaveChanges();

        public Task<IDbContextTransaction> BeginTransactionAsync() => context.Database.BeginTransactionAsync();
    }
}
