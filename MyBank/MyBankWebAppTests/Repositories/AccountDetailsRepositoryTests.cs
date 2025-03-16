using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MyBankWebApp.Data;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories;
using MyBankWebApp.Repositories.Abstractions;
using System.Linq;

namespace MyBankWebAppTests.Repositories
{
    [TestClass()]
    public class AccountDetailsRepositoryTests
    {
        private ApplicationDbContext context;
        private IAccountDetailsRepository sut;

        [TestMethod()]
        public void AnyByIdAsync_ReturnsCorrect()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void BeginTransactionAsync_ReturnsTransaction()
        {
            //Act
            var result = sut.BeginTransactionAsync();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<IDbContextTransaction>(result);
        }

        [TestMethod()]
        public void GetAccountByIbanAsync_ReturnsCorrect()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public void GetByIdAsync_ReturnsCorrect()
        {
            throw new NotImplementedException();
        }

        [TestMethod()]
        public async Task AddRecordToDatabase_Success()
        {
            //Arrange
            const string input_Iban = "121241234123412341234";
            var input_AccountDetail = new AccountDetail()
            {
                CountryCode = "PL",
                IBAN = input_Iban,
                Balance = 1000,
            };

            //Act
            await sut.AddAsync(input_AccountDetail);
            await sut.SaveAsync();

            //Assert
            AccountDetail result = await context.AccountDetails.FirstOrDefaultAsync(account => account.IBAN == input_Iban);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<AccountDetail>(result);
            Assert.AreEqual(input_AccountDetail, result);
        }

        [TestInitialize]
        public async Task XInitialize()
        {
            context = await GetDbContext();
            sut = new AccountDetailsRepository(context);
        }

        private static async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            await context.Database.EnsureCreatedAsync();

            if (!await context.AccountDetails.AnyAsync())
            {
                context.AccountDetails.Add(new AccountDetail
                {
                    CountryCode = "PL",
                    UserId = 1,
                    IBAN = "61109010140000071219812874",
                    Balance = 2500.00m
                });
                await context.SaveChangesAsync();
            }
            return context;
        }
    }
}