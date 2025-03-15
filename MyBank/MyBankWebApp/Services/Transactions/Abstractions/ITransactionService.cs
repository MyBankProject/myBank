using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Services.Transactions.Abstractions
{
    public interface ITransactionService
    {
        Task AddTransactionAsync(NewTransactionViewModel newTransaction);
    }
}