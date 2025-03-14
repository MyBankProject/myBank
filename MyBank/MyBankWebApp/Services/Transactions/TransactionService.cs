using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using MyBankWebApp.DTOs;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Transactions.Abstractions;

namespace MyBankWebApp.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly IAccountDetailsRepository accountDetailsRepository;
        private readonly IMapper mapper;
        private readonly ITransactionRepository transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountDetailsRepository accountDetailsRepository, IMapper mapper)
        {
            this.transactionRepository = transactionRepository;
            this.accountDetailsRepository = accountDetailsRepository;
            this.mapper = mapper;
        }

        public async Task AddTransactionAsync(NewTransactionDto newTransaction)
        {
            var reciverAccount = await accountDetailsRepository.GetAccountByIbanAsync(newTransaction.ReciverIBAN);
            var senderAccount =  await accountDetailsRepository.GetByIdAsync(newTransaction.SenderId);
            ValidateTransaction(senderAccount, newTransaction);
            using IDbContextTransaction dbTransaction = await transactionRepository.BeginTransactionAsync();
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

        private async Task ProcessTransaction(AccountDetail senderAccount, AccountDetail reciverAccount, NewTransactionDto newTransaction)
        {
            Transaction transaction = CreateTransaction(senderAccount, reciverAccount, newTransaction);
            UpdateBalanceForBothSides(senderAccount, reciverAccount, newTransaction);
            await transactionRepository.AddAsync(transaction);
            await transactionRepository.SaveAsync();
        }
    }
}