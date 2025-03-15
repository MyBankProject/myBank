using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Mappers;
using MyBankWebApp.Models;
using MyBankWebApp.Models.Abstractions;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.ViewModels;
using System.Data.Common;

namespace MyBankWebApp.Services.Transactions
{
    [TestClass]
    public class TransactionServiceTests
    {
        private const string default_Iban = "PL12 1234 1234 1234 1234 1234";
        private const string IbanNumbersOnly = "1212341234123412341234";
        private Mock<IAccountDetailsRepository> accountDetailsRepository;
        private IMapper mapper;
        private Mock<IAccountDetail> reciver;
        private Mock<IAccountDetail> sender;
        private TransactionService sut;
        private Mock<ITransactionRepository> transactionRepository;

        [TestMethod]
        public async Task AddTransactionAsync_NotEnoughBalance_ThrowsException()
        {
            //Arrange
            sender.Setup(_ => _.Balance).Returns(100);

            const int senderId = 1;
            var transactionVM = new NewTransactionViewModel()
            {
                ReciverIBAN = default_Iban,
                Amount = 123.12m,
                SenderId = senderId
            };

            //Act
            await Assert.ThrowsExceptionAsync<LackOfFundsException>(() => sut.AddTransactionAsync(transactionVM));

            //Assert
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(It.IsAny<string>()), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(IbanNumbersOnly), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(It.IsAny<int>(), null), Times.Once);
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(senderId, null), Times.Once);
        }

        [TestMethod]
        public async Task AddTransactionAsync_ErrorDuringTransaction_RollsBackAndThrowsException()
        {
            //Arrange
            sender.SetupProperty(_ => _.Balance, 100);
            reciver.SetupProperty(_ => _.Balance, 100);
            var transaction = new Mock<IDbContextTransaction>();
            transactionRepository.Setup(_ => _.BeginTransactionAsync()).ReturnsAsync(transaction.Object);

            const int senderId = 1;
            var transactionVM = new NewTransactionViewModel()
            {
                ReciverIBAN = default_Iban,
                Amount = 100.00m,
                SenderId = senderId
            };

            //Act
            await Assert.ThrowsExceptionAsync<NullReferenceException>(() => sut.AddTransactionAsync(transactionVM));

            //Assert
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(It.IsAny<string>()), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(IbanNumbersOnly), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(It.IsAny<int>(), null), Times.Once);
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(senderId, null), Times.Once);
            transactionRepository.Verify(_ => _.BeginTransactionAsync(), Times.Once);
            transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task AddTransactionAsync_CreatesTransactionCorrectly()
        {
            //Arrange
            sender.SetupProperty(_ => _.Balance, 100);
            reciver.SetupProperty(_ => _.Balance, 100);
            var transaction = new Mock<IDbContextTransaction>();
            transactionRepository.Setup(_ => _.BeginTransactionAsync()).ReturnsAsync(transaction.Object);

            const int senderId = 1;
            var transactionVM = new NewTransactionViewModel()
            {
                ReciverIBAN = default_Iban,
                Amount = 100.00m,
                Description = "description",
                ReciverName = "Reciver Name",
                TransferDate = DateTime.Now,
                SenderId = senderId
            };

            //Act
            await sut.AddTransactionAsync(transactionVM));

            //Assert
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(It.IsAny<string>()), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(IbanNumbersOnly), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(It.IsAny<int>(), null), Times.Once);
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(senderId, null), Times.Once);
            transactionRepository.Verify(_ => _.BeginTransactionAsync(), Times.Once);
            sender.VerifySet(_ => _.Balance = 0.00m, Times.Once());
            reciver.VerifySet(_ => _.Balance = 200.00m, Times.Once());
            transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.Fail("Dokończyć pisać test");
        }

        [TestInitialize]
        public void XInitialize()
        {
            transactionRepository = new Mock<ITransactionRepository>();
            accountDetailsRepository = new Mock<IAccountDetailsRepository>();
            reciver = new Mock<IAccountDetail>();
            sender = new Mock<IAccountDetail>();
            accountDetailsRepository.Setup(_ => _.GetByIdAsync(It.IsAny<int>(), null)).ReturnsAsync(sender.Object);
            accountDetailsRepository.Setup(_ => _.GetAccountByIbanAsync(It.IsAny<string>())).ReturnsAsync(reciver.Object);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NewTransactionVMToTransactionMapper>();
            });
            mapper = config.CreateMapper();

            sut = new TransactionService(transactionRepository.Object, accountDetailsRepository.Object, mapper);
        }
    }
}
