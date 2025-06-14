using Microsoft.AspNetCore.Identity;
using MyBankWebApp.Models;
using MyBankWebApp.Models.Users;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Data
{
    public static class Seed
    {
        private static IEnumerable<User> Users
        {
            get
            {
                var passwordHasher = new PasswordHasher<User>();
                var user1 = new User()
                {
                    FirstName = "a",
                    LastName = "a",
                    Email = "a@a.pl",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1990, 1, 1), DateTimeKind.Utc),
                    Nationality = "PL",
                    RoleId = 1,
                    AccountId = 1,
                };
                user1.PasswordHash = passwordHasher.HashPassword(user1, "a");

                var user2 = new User()
                {
                    FirstName = "b",
                    LastName = "b",
                    Email = "b@b.pl",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1985, 5, 20), DateTimeKind.Utc),
                    Nationality = "PL",
                    RoleId = 2,
                    AccountId = 2,
                };
                user2.PasswordHash = passwordHasher.HashPassword(user2, "bbbbbb");

                var user3 = new User
                {
                    Email = "c@c.pl",
                    FirstName = "c",
                    LastName = "c",
                    Nationality = "PL",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1992, 11, 15), DateTimeKind.Utc),
                    RoleId = 3,
                    AccountId = 3,
                };
                user3.PasswordHash = passwordHasher.HashPassword(user3, "cccccc");

                var user4 = new User
                {
                    Email = "peter.parker@example.com",
                    FirstName = "Peter",
                    LastName = "Parker",
                    Nationality = "PL",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1995, 8, 10), DateTimeKind.Utc),
                    RoleId = 1,
                    AccountId = 4
                };
                user4.PasswordHash = passwordHasher.HashPassword(user4, "dddddd");

                var user5 = new User
                {
                    Email = "anna.nowak@example.com",
                    FirstName = "Anna",
                    LastName = "Nowak",
                    Nationality = "PL",
                    DateOfBirth = DateTime.SpecifyKind(new DateTime(1988, 3, 3), DateTimeKind.Utc),
                    RoleId = 1,
                    AccountId = 5
                };
                user5.PasswordHash = passwordHasher.HashPassword(user5, "eeeeee");

                return
                    [
                        user1,
                        user2,
                        user3,
                        user4,
                        user5
                    ];
            }
        }

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

            if (!context.Users.Any())
            {
                IEnumerable<User> users = Users;
                context.Users.AddRange(users);
                context.SaveChanges();
            }
        }

        private static IEnumerable<Account> GetAccountDetails() =>
            [
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
            ];

        private static IEnumerable<Role> GetRoles() =>
            [
                new Role() { Name = "User" },
                new Role() { Name = "Manager" },
                new Role() { Name = "Admin" }
            ];

        private static IEnumerable<Transaction> GetTransactions() =>
        [
            new Transaction()
            {
                Amount = 100.00m,
                CreationTime = DateTime.Now.AddDays(-5),
                Description = "Payment for services",
                ReceiverId = 1,
                SenderId = 2,
                StatusId = (int)TransactionStatuses.Completed,
                TransactionTypeId = (int)TransactionTypes.Transfer
            },
            new Transaction()
            {
                Amount = 50.50m,
                CreationTime = DateTime.Now.AddDays(-3),
                Description = "Invoice payment",
                ReceiverId = 3,
                SenderId = 4,
                StatusId = (int)TransactionStatuses.Completed,
                TransactionTypeId = (int)TransactionTypes.Transfer
            },
            new Transaction()
            {
                Amount = 200.00m,
                CreationTime = DateTime.Now.AddDays(-7),
                Description = "Refund for purchase",
                ReceiverId = 5,
                SenderId = 1,
                StatusId = (int)TransactionStatuses.Completed,
                TransactionTypeId = (int)TransactionTypes.Transfer
            },
            new Transaction()
            {
                Amount = 120.00m,
                CreationTime = DateTime.Now.AddDays(-10),
                Description = "Monthly subscription",
                ReceiverId = 2,
                SenderId = 3,
                StatusId = (int)TransactionStatuses.Failed,
                TransactionTypeId = (int)TransactionTypes.Deposit
            },
            new Transaction()
            {
                Amount = 300.00m,
                CreationTime = DateTime.Now.AddDays(-2),
                Description = "Salary payment",
                ReceiverId = 4,
                SenderId = 5,
                StatusId = (int)TransactionStatuses.Completed,
                TransactionTypeId = (int)TransactionTypes.Deposit
            }
        ];
    }
}