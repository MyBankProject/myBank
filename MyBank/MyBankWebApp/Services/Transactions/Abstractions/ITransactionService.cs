using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Services.Transactions.Abstractions
{
    public interface ITransactionService
    {
        Task AddDepositAsync(DepositViewModel newDeposit);

        Task AddTransactionAsync(NewTransactionViewModel newTransaction);
    }
}