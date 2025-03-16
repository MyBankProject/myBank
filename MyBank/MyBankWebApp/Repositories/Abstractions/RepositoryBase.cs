using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyBankWebApp.Data;

namespace MyBankWebApp.Repositories.Abstractions
{
    public abstract class RepositoryBase<T>(ApplicationDbContext context) : IRepositoryBase<T> where T : class
    {
        protected readonly ApplicationDbContext context = context;
        protected DbSet<T> DbSet => context.Set<T>();

        public async Task AddAsync(T transaction) => await DbSet.AddAsync(transaction);

        public async Task<IDbContextTransaction> BeginTransactionAsync() => await context.Database.BeginTransactionAsync();

        public async Task SaveAsync() => await context.SaveChangesAsync();
    }
}