using MyBankWebApp.Models;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Data
{
    public static class Seed
    {
        public static void SeedData(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();

                context.Database.EnsureCreated();

                if (!context.AccountDetails.Any())
                {
                    context.AccountDetails.AddRange(new List<AccountDetail>()
                    {
                        new AccountDetail()
                        {
                            IBAN = "PL61109010140000071219812874",
                            CountryCode = "PL",
                            Balance = 1500.75m
                        },
                        new AccountDetail()
                         {
                             IBAN = "PL27114020040000300201355387",
                             CountryCode = "PL",
                             Balance = 240.50m
                         },
                         new AccountDetail()
                         {
                             IBAN = "PL46116022020000000231710798",
                             CountryCode = "PL",
                             Balance = 320000.00m
                         },
                         new AccountDetail()
                         {
                             IBAN = "PL10105000997603123456789123",
                             CountryCode = "PL",
                             Balance = 50.25m
                         },
                         new AccountDetail()
                         {
                             IBAN = "PL27113000000000123456789123",
                             CountryCode = "PL",
                             Balance = 785000000.90m
                         }
                    });
                    context.SaveChanges();
                }

                if (!context.Transactions.Any())
                {
                    context.Transactions.AddRange(new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Amount = 100.00m,
                            CreationTime = DateTime.Now.AddDays(-5),
                            Description = "Payment for services",
                            Reciver = 1,
                            Sender = 2,
                            Status = TransactionStatus.Completed,
                            TransactionType = TransactionTypes.Transfer
                        },
                        new Transaction()
                        {
                            Amount = 50.50m,
                            CreationTime = DateTime.Now.AddDays(-3),
                            Description = "Invoice payment",
                            Reciver = 3,
                            Sender = 4,
                            Status = TransactionStatus.Completed,
                            TransactionType = TransactionTypes.Transfer
                        },
                        new Transaction()
                        {
                            Amount = 200.00m,
                            CreationTime = DateTime.Now.AddDays(-7),
                            Description = "Refund for purchase",
                            Reciver = 5,
                            Sender = 1,
                            Status = TransactionStatus.Completed,
                            TransactionType = TransactionTypes.Transfer
                        },
                        new Transaction()
                        {
                            Amount = 120.00m,
                            CreationTime = DateTime.Now.AddDays(-10),
                            Description = "Monthly subscription",
                            Reciver = 2,
                            Sender = 3,
                            Status = TransactionStatus.Failed,
                            TransactionType = TransactionTypes.Deposit
                        },
                        new Transaction()
                        {
                            Amount = 300.00m,
                            CreationTime = DateTime.Now.AddDays(-2),
                            Description = "Salary payment",
                            Reciver = 4,
                            Sender = 5,
                            Status = TransactionStatus.Completed,
                            TransactionType = TransactionTypes.Deposit
                        }
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}
