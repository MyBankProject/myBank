using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using MyBankWebApp.Data;
using MyBankWebApp.DTOs;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;

namespace MyBankWebApp.Services.TransactionServices
{
    public class TransactionService
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public TransactionService(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<bool> AddTransactionAsync(NewTransactionDto newTransaction)
        {
            AccountDetail? senderAccount = context.AccountDetails.FirstOrDefault(account => account.UserId == newTransaction.SenderId);
            AccountDetail? reciverAccount = context.AccountDetails.FirstOrDefault(account => account.IBAN == newTransaction.ReciverIBAN);
            ValidateTransaction(newTransaction, senderAccount, reciverAccount);
            using IDbContextTransaction dbTransaction = await context.Database.BeginTransactionAsync();
            try
            {
                UpdateBalanceForBothSides(newTransaction, senderAccount, reciverAccount);
                Transaction transaction = CreateTransaction(newTransaction, senderAccount, reciverAccount);
                await context.Transactions.AddAsync(transaction);
                await dbTransaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return false;
            }
        }

        private static void UpdateBalanceForBothSides(NewTransactionDto newTransaction, AccountDetail? senderAccount, AccountDetail? reciverAccount)
        {
            senderAccount.Balance -= newTransaction.Amount;
            reciverAccount.Balance += newTransaction.Amount;
        }

        private static void ValidateTransaction(NewTransactionDto newTransaction, AccountDetail? senderAccount, AccountDetail? reciverAccount)
        {
            if (reciverAccount == null || senderAccount == null)
            {
                throw new UserNotFoundException("User do not exist");
            }
            if (senderAccount.Balance < newTransaction.Amount)
            {
                throw new LackOfFundsException("Not enough funds");
            }
        }

        private Transaction CreateTransaction(NewTransactionDto newTransaction, AccountDetail? senderAccount, AccountDetail? reciverAccount)
        {
            Transaction transaction = mapper.Map<Transaction>(newTransaction);
            //TODO: Trzeba kogoś dopytać o to czy trzeba wypełniać te property. EF sam tego nie zrobi?
            transaction.SenderAccountDetails = senderAccount;
            transaction.ReciverAccountDetails = reciverAccount;
            transaction.Reciver = reciverAccount.UserId;
            transaction.Status = Enums.TransactionStatus.Completed;
            return transaction;
        }
    }
}
