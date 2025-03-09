using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using MyBankWebApp.Data;
using MyBankWebApp.DTOs;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Services.Transactions.Abstractions;

namespace MyBankWebApp.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public TransactionService(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task AddTransactionAsync(NewTransactionDto newTransaction)
        {
            AccountDetail senderAccount = GetAccountById(newTransaction);
            AccountDetail reciverAccount = GetAccountByIban(newTransaction);
            ValidateTransaction(senderAccount, newTransaction);
            using IDbContextTransaction dbTransaction = await context.Database.BeginTransactionAsync();
            try
            {
                ProcessTransaction(senderAccount, reciverAccount, newTransaction);
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        private static void UpdateBalanceForBothSides(AccountDetail senderAccount, AccountDetail reciverAccount, NewTransactionDto newTransaction)
        {
            senderAccount.Balance -= newTransaction.Amount;
            reciverAccount.Balance += newTransaction.Amount;
        }

        private static void ValidateTransaction(AccountDetail senderAccount, NewTransactionDto newTransaction)
        {
            if (senderAccount.Balance < newTransaction.Amount)
            {
                throw new LackOfFundsException("Not enough funds");
            }
        }

        private Transaction CreateTransaction(AccountDetail senderAccount, AccountDetail reciverAccount, NewTransactionDto newTransaction)
        {
            Transaction transaction = mapper.Map<Transaction>(newTransaction);
            //TODO: Trzeba kogoś dopytać o to czy trzeba wypełniać te property. EF sam tego nie zrobi?
            transaction.SenderAccountDetails = senderAccount;
            transaction.ReciverAccountDetails = reciverAccount;
            transaction.Reciver = reciverAccount.UserId;
            transaction.Status = Enums.TransactionStatus.Completed;
            return transaction;
        }

        private AccountDetail GetAccountByIban(NewTransactionDto newTransaction) =>
            context.AccountDetails.FirstOrDefault(account => account.IBAN == newTransaction.ReciverIBAN)
                ?? throw new UserNotFoundException("Reciver not found");

        private AccountDetail GetAccountById(NewTransactionDto newTransaction) =>
            context.AccountDetails.FirstOrDefault(account => account.UserId == newTransaction.SenderId)
                ?? throw new UserNotFoundException("Sender not found");

        private void ProcessTransaction(AccountDetail senderAccount, AccountDetail reciverAccount, NewTransactionDto newTransaction)
        {
            Transaction transaction = CreateTransaction(senderAccount, reciverAccount, newTransaction);
            UpdateBalanceForBothSides(senderAccount, reciverAccount, newTransaction);
            context.Transactions.Add(transaction);
            context.SaveChanges();
        }
    }
}