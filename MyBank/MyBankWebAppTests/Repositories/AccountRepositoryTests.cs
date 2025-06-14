using Microsoft.EntityFrameworkCore;
using MyBankWebApp.Data;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories;

namespace MyBankWebAppTests.Repositories
{
    [TestClass()]
    public class AccountRepositoryTests
    {
        private const string default_Iban = "61109010140000071219812874";
        private const int default_Id = 1;

        private readonly Account input_AccountRecord = new()
        {
            CountryCode = "PL",
            Id = default_Id,
            IBAN = default_Iban,
            Balance = 2500.00m
        };

        private ApplicationDbContext context;
        private AccountRepository sut;

        [TestMethod()]
        public async Task AddRecordToDatabase_Success()
        {
            //Arrange
            const string input_Iban = "121241234123412341234";
            var input_Account = new Account()
            {
                CountryCode = "PL",
                IBAN = input_Iban,
                Balance = 1000,
            };

            //Act
            await sut.AddAsync(input_Account);
            await sut.SaveAsync();

            //Assert
            Account result = await context.Accounts.FirstOrDefaultAsync(account => account.IBAN == input_Iban);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType<Account>(result);
            Assert.AreEqual(input_Account, result);
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
            Account result = await sut.GetAccountByIbanAsync(default_Iban);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreSame(input_AccountRecord, result);
        }

        [TestMethod()]
        public async Task GetAccountByIbanAsync_DbDoesNotContainsIban_ReturnsNull()
        {
            //Act
            Account result = await sut.GetAccountByIbanAsync("111111111111111111");

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public async Task GetByIdAsync_ReturnsCorrect()
        {
            //Act
            Account result = await sut.GetByIdAsync(default_Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreSame(input_AccountRecord, result);
        }

        [TestInitialize]
        public async Task XInitialize()
        {
            context = await GetDbContext();
            sut = new AccountRepository(context);
        }

        private async Task<ApplicationDbContext> GetDbContext()
        {
            DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);
            await context.Database.EnsureCreatedAsync();

            if (!await context.Accounts.AnyAsync())
            {
                context.Accounts.Add(input_AccountRecord);
                await context.SaveChangesAsync();
            }
            return context;
        }
    }
}