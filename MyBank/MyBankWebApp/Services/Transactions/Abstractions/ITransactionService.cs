﻿using MyBankWebApp.DTOs;

namespace MyBankWebApp.Services.Transactions.Abstractions
{
    public interface ITransactionService
    {
        Task<bool> AddTransactionAsync(NewTransactionDto newTransaction);
    }
}