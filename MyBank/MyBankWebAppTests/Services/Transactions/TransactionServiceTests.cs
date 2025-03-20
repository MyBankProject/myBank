using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Mappers;
using MyBankWebApp.Models;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Transactions;
using MyBankWebApp.ViewModels;

namespace MyBankWebAppTests.Services.Transactions
{
    [TestClass]
    public class TransactionServiceTests
    {
        private const string default_Iban = "PL12 1234 1234 1234 1234 1234";
        private const string IbanNumbersOnly = "1212341234123412341234";
        private const string default_CountryCode = "PL";
        private Mock<IAccountDetailsRepository> accountDetailsRepository;
        private IMapper mapper;
        private Account reciver;
        private Account sender;
        private TransactionService sut;
        private Mock<ITransactionRepository> transactionRepository;

        [TestMethod]
        public async Task AddTransactionAsync_CreatesTransactionCorrectly()
        {
            //Arrange
            var transaction = new Mock<IDbContextTransaction>();
            transactionRepository.Setup(_ => _.BeginTransactionAsync()).ReturnsAsync(transaction.Object);
            Transaction resultTransaction = null;
            transactionRepository.Setup(_ => _.AddAsync(It.IsAny<Transaction>())).Callback<Transaction>(tsct => resultTransaction = tsct);

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
            await sut.AddTransactionAsync(transactionVM);

            //Assert
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(It.IsAny<string>()), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(IbanNumbersOnly), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(It.IsAny<int>(), null), Times.Once);
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(senderId, null), Times.Once);
            transactionRepository.Verify(_ => _.BeginTransactionAsync(), Times.Once);
            Assert.AreEqual(0.00m, sender.Balance);
            Assert.AreEqual(200.00m, reciver.Balance);
            transaction.Verify(_ => _.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            transactionRepository.Verify(_ => _.AddAsync(It.IsAny<Transaction>()), Times.Once);
            transactionRepository.Verify(_ => _.SaveAsync(), Times.Once);
            Assert.IsNotNull(resultTransaction);
            Assert.AreSame(sender, resultTransaction.SenderAccount);
        }

        [TestMethod]
        public async Task AddTransactionAsync_ErrorDuringTransaction_RollsBackAndThrowsException()
        {
            //Arrange
            var transaction = new Mock<IDbContextTransaction>();
            transactionRepository.Setup(_ => _.BeginTransactionAsync()).ReturnsAsync(transaction.Object);
            transactionRepository.Setup(_ => _.AddAsync(It.IsAny<Transaction>())).ThrowsAsync(new Exception());

            const int senderId = 1;
            var transactionVM = new NewTransactionViewModel()
            {
                ReciverIBAN = default_Iban,
                Amount = 100.00m,
                SenderId = senderId
            };

            //Act
            await Assert.ThrowsExceptionAsync<Exception>(() => sut.AddTransactionAsync(transactionVM));

            //Assert
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(It.IsAny<string>()), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetAccountByIbanAsync(IbanNumbersOnly), Times.Once());
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(It.IsAny<int>(), null), Times.Once);
            accountDetailsRepository.Verify(_ => _.GetByIdAsync(senderId, null), Times.Once);
            transactionRepository.Verify(_ => _.BeginTransactionAsync(), Times.Once);
            transaction.Verify(_ => _.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task AddTransactionAsync_NotEnoughBalance_ThrowsException()
        {
            //Arrange
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

        [TestInitialize]
        public void XInitialize()
        {
            transactionRepository = new Mock<ITransactionRepository>();
            accountDetailsRepository = new Mock<IAccountDetailsRepository>();
            reciver = new Account() 
            {
                Balance = 100,
                IBAN = default_Iban,
                CountryCode = default_CountryCode
            };
            sender = new Account() 
            {
                Balance = 100,
                IBAN = default_Iban,
                CountryCode = default_CountryCode
            };
            accountDetailsRepository.Setup(_ => _.GetByIdAsync(It.IsAny<int>(), null)).ReturnsAsync(sender);
            accountDetailsRepository.Setup(_ => _.GetAccountByIbanAsync(It.IsAny<string>())).ReturnsAsync(reciver);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<NewTransactionVMToTransactionMapper>();
            });
            mapper = config.CreateMapper();

            sut = new TransactionService(transactionRepository.Object, accountDetailsRepository.Object, mapper);
        }
    }
}