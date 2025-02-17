using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Models;

namespace MyBankWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AccountDetail> AccountDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
