using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Data;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories;

namespace MyBankWebAppTests.Repositories
{
    [TestClass()]
    public class AccountDetailsRepositoryTests
    {
        private const string default_Iban = "61109010140000071219812874";
        private const int default_Id = 1;

        private readonly Account input_AccountDetailRecord = new()
        {
            CountryCode = "PL",
            Id = default_Id,
            IBAN = default_Iban,
            Balance = 2500.00m
        };

        private ApplicationDbContext context;
        private AccountDetailsRepository sut;

        [TestMethod()]
        public async Task AddRecordToDatabase_Success()
        {
            //Arrange
            const string input_Iban = "121241234123412341234";
            var input_AccountDetail = new Account()
            {
                CountryCode = "PL",
                IBAN = input_Iban,
                Balance = 1000,
            };

            //Act
            await sut.AddAsync(input_AccountDetail);
            await sut.SaveAsync();

            //Assert
            Account result = await context.Accounts.FirstOrDefaultAsync(account => account.IBAN == input_Iban);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<Account>(result);
            Assert.AreEqual(input_AccountDetail, result);
        }

        [TestMethod()]
        public async Task AnyByIdAsync_False_ReturnsCorrect()
        {
            //Act
            bool result = await sut.AnyByIdAsync(101);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsFalse(result);
        }

        [TestMethod()]
        public async Task AnyByIdAsync_True_ReturnsCorrect()
        {
            //Act
            bool result = await sut.AnyByIdAsync(default_Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public async Task GetAccountByIbanAsync_DbContainsIban_ReturnsCorrect()
        {
            //Act
            var result = await sut.GetAccountByIbanAsync(default_Iban);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreSame(input_AccountDetailRecord, result);
        }

        [TestMethod()]
        public async Task GetAccountByIbanAsync_DbDoesNotContainsIban_ReturnsNull()
        {
            //Act
            var result = await sut.GetAccountByIbanAsync("111111111111111111");

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public async Task GetByIdAsync_ReturnsCorrect()
        {
            //Act
            var result = await sut.GetByIdAsync(default_Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreSame(input_AccountDetailRecord, result);
        }

        [TestInitialize]
        public async Task XInitialize()
        {
            context = await GetDbContext();
            sut = new AccountDetailsRepository(context);
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);
            await context.Database.EnsureCreatedAsync();

            if (!await context.Accounts.AnyAsync())
            {
                context.Accounts.Add(input_AccountDetailRecord);
                await context.SaveChangesAsync();
            }
            return context;
        }
    }
}