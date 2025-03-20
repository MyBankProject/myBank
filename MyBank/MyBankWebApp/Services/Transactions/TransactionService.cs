using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Transactions.Abstractions;
using MyBankWebApp.ViewModels;
using System.Text.RegularExpressions;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountRepository accountDetailsRepository;
        private readonly IMapper mapper;
        private readonly ITransactionRepository transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountDetailsRepository, IMapper mapper)
        {
            this.transactionRepository = transactionRepository;
            this.accountDetailsRepository = accountDetailsRepository;
            this.mapper = mapper;
        }

        public async Task AddTransactionAsync(NewTransactionViewModel newTransaction)
        {
            string filteredIban = Regex.Replace(newTransaction.ReciverIBAN, @"\D", "");
            Account? reciverAccount = await accountDetailsRepository.GetAccountByIbanAsync(filteredIban);
            Account? senderAccount = await accountDetailsRepository.GetByIdAsync(newTransaction.SenderId);
            ValidateTransaction(senderAccount, reciverAccount, newTransaction);
            using IDbContextTransaction dbTransaction = await transactionRepository.BeginTransactionAsync();
            try
            {
                await ProcessTransaction(senderAccount!, reciverAccount!, newTransaction);
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        private static void UpdateBalanceForBothSides(Account senderAccount, Account reciverAccount, NewTransactionViewModel newTransaction)
        {
            senderAccount.Balance -= newTransaction.Amount;
            reciverAccount.Balance += newTransaction.Amount;
        }

        private static void ValidateTransaction(Account? senderAccount, Account? reciverAccount, NewTransactionViewModel newTransaction)
        {
            if (senderAccount == null)
                throw new UserNotFoundException("Reciver not found");

            if (reciverAccount == null)
                throw new UserNotFoundException("Sender not found");

            if (senderAccount.Balance < newTransaction.Amount)
                throw new LackOfFundsException("Not enough funds");
        }

        private Transaction CreateTransaction(Account senderAccount, Account reciverAccount, NewTransactionViewModel newTransaction)
        {
            Transaction transaction = mapper.Map<Transaction>(newTransaction);
            //TODO: Muszę kogoś dopytać o to czy trzeba wypełniać te property. EF sam tego nie zrobi?
            transaction.ReceiverId = reciverAccount.Id;
            transaction.SenderId = senderAccount.Id;
            transaction.StatusId = Enum.IsDefined(typeof(Enums.TransactionStatuses), Enums.TransactionStatuses.Completed)
                        ? (int)Enums.TransactionStatuses.Completed
                        : default;
            transaction.TransactionTypeId = Enum.IsDefined(typeof(TransactionTypes), Enums.TransactionTypes.Transfer)
                        ? (int)Enums.TransactionTypes.Transfer
                        : default;
            return transaction;
        }

        private async Task ProcessTransaction(Account senderAccount, Account reciverAccount, NewTransactionViewModel newTransaction)
        {
            Transaction transaction = CreateTransaction(senderAccount, reciverAccount, newTransaction);
            UpdateBalanceForBothSides(senderAccount, reciverAccount, newTransaction);
            await transactionRepository.AddAsync(transaction);
            await transactionRepository.SaveAsync();
        }
    }
}