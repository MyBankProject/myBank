using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Models;

namespace MyBankWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AccountDetail> AccountDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.SenderAccountDetails)
                .WithMany(a => a.SentTransactions)
                .HasForeignKey(t => t.Sender)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ReciverAccountDetails)
                .WithMany(a => a.RecivedTransactions)
                .HasForeignKey(t => t.Reciver)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
