using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Data;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;

namespace MyBankWebApp.Repositories
{
    internal class TransactionRepository(ApplicationDbContext context) : RepositoryBase<Transaction>(context), ITransactionRepository
    {
    }
}