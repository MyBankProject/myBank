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
        private const string DEPOSIT_DESCRIPTION_STRING = "Deposit";
        private const int DEPOSIT_SENDER_DEFAULT = 1;
        private readonly IAccountRepository accountDetailsRepository;
        private readonly IMapper mapper;
        private readonly ITransactionRepository transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountDetailsRepository, IMapper mapper)
        {
            this.transactionRepository = transactionRepository;
            this.accountDetailsRepository = accountDetailsRepository;
            this.mapper = mapper;
        }

        public async Task AddDepositAsync(DepositViewModel newDeposit)
        {
            string filteredIban = Regex.Replace(newDeposit.ReceiverIBAN, @"\D", "");
            Account? reciverAccount = await accountDetailsRepository.GetAccountByIbanAsync(filteredIban);
            if (reciverAccount == null)
            {
                throw new UserNotFoundException("Reciver not found");
            }

            using IDbContextTransaction dbTransaction = await transactionRepository.BeginTransactionAsync();
            try
            {
                await ProcessDeposit(reciverAccount!, newDeposit);
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task AddTransactionAsync(NewTransactionViewModel newTransaction)
        {
            string filteredIban = Regex.Replace(newTransaction.ReceiverIBAN, @"\D", "");
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

        private Transaction CreateTransaction(Account reciverAccount, DepositViewModel newDeposit)
        {
            Transaction transaction = mapper.Map<Transaction>(newDeposit);
            transaction.ReceiverId = reciverAccount.Id;
            transaction.Description = DEPOSIT_DESCRIPTION_STRING;
            transaction.SenderId  = DEPOSIT_SENDER_DEFAULT;
            transaction.StatusId = Enum.IsDefined(typeof(Enums.TransactionStatuses), Enums.TransactionStatuses.Completed)
                        ? (int)Enums.TransactionStatuses.Completed
                        : default;
            transaction.TransactionTypeId = Enum.IsDefined(typeof(TransactionTypes), Enums.TransactionTypes.Transfer)
                        ? (int)Enums.TransactionTypes.Deposit
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

        private async Task ProcessDeposit(Account reciverAccount, DepositViewModel newDeposit)
        {
            Transaction transaction = CreateTransaction(reciverAccount, newDeposit);
            reciverAccount.Balance += newDeposit.Amount;
            await transactionRepository.AddAsync(transaction);
            await transactionRepository.SaveAsync();
        }
    }
}