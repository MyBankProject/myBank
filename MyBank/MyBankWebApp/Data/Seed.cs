using MyBankWebApp.Entities;
using MyBankWebApp.Models;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Data
{
    public static class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using IServiceScope serviceScope = applicationBuilder.ApplicationServices.CreateScope();
            ApplicationDbContext context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>() ?? 
                throw new InvalidOperationException("Database context is not initialized.");
            context?.Database.EnsureCreated();
            if (!context!.Roles.Any())
            {
                IEnumerable<Role> roles = GetRoles();
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            if (!context.Accounts.Any())
            {
                IEnumerable<Account> accountDetails = GetAccountDetails();
                context.Accounts.AddRange(accountDetails);
                context.SaveChanges();
            }

            if (!context.Transactions.Any())
            {
                IEnumerable<Transaction> transactions = GetTransactions();
                context.Transactions.AddRange(transactions);
                context.SaveChanges();
            }
        }

        private static IEnumerable<Transaction> GetTransactions()
        {
            return new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Amount = 100.00m,
                            CreationTime = DateTime.Now.AddDays(-5),
                            Description = "Payment for services",
                            ReceiverId = 1,
                            SenderId = 2,
                            Status = TransactionStatus.Completed,
                            TransactionType = TransactionTypes.Transfer
                        },
                        new Transaction()
                        {
                            Amount = 50.50m,
                            CreationTime = DateTime.Now.AddDays(-3),
                            Description = "Invoice payment",
                            ReceiverId = 3,
                            SenderId = 4,
                            Status = TransactionStatus.Completed,
                            TransactionType = TransactionTypes.Transfer
                        },
                        new Transaction()
                        {
                            Amount = 200.00m,
                            CreationTime = DateTime.Now.AddDays(-7),
                            Description = "Refund for purchase",
                            ReceiverId = 5,
                            SenderId = 1,
                            Status = TransactionStatus.Completed,
                            TransactionType = TransactionTypes.Transfer
                        },
                        new Transaction()
                        {
                            Amount = 120.00m,
                            CreationTime = DateTime.Now.AddDays(-10),
                            Description = "Monthly subscription",
                            ReceiverId = 2,
                            SenderId = 3,
                            Status = TransactionStatus.Failed,
                            TransactionType = TransactionTypes.Deposit
                        },
                        new Transaction()
                        {
                            Amount = 300.00m,
                            CreationTime = DateTime.Now.AddDays(-2),
                            Description = "Salary payment",
                            ReceiverId = 4,
                            SenderId = 5,
                            Status = TransactionStatus.Completed,
                            TransactionType = TransactionTypes.Deposit
                        }
                    };
        }

        private static IEnumerable<Account> GetAccountDetails()
        {
            return new List<Account>()
                    {
                        new Account()
                        {
                            IBAN = "61109010140000071219812874",
                            CountryCode = "PL",
                            Balance = 1500.75m
                        },
                        new Account()
                         {
                             IBAN = "27114020040000300201355387",
                             CountryCode = "PL",
                             Balance = 240.50m
                         },
                         new Account()
                         {
                             IBAN = "46116022020000000231710798",
                             CountryCode = "PL",
                             Balance = 320000.00m
                         },
                         new Account()
                         {
                             IBAN = "10105000997603123456789123",
                             CountryCode = "PL",
                             Balance = 50.25m
                         },
                         new Account()
                         {
                             IBAN = "27113000000000123456789123",
                             CountryCode = "PL",
                             Balance = 785000000.90m
                         }
                    };
        }

        private static IEnumerable<Role> GetRoles()
        {
            return new List<Role>()
            {
                new Role() { Name = "User" },
                new Role() { Name = "Manager" },
                new Role() { Name = "Admin" }
            };
        }
    }
}
