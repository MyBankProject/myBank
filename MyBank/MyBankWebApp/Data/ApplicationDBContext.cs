using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Entities;
using MyBankWebApp.Models;

namespace MyBankWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<AccountDetail> AccountDetails { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(u => u.Name)
                .IsRequired();

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
