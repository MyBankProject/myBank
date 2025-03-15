using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyBankWebApp.Data;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Models.Abstractions;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Transactions;
using MyBankWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankWebApp.Services.Transactions
{
    [TestClass]
    public class TransactionServiceTests
    {
        private const string default_Iban = "PL12 1234 1234 1234 1234 1234";
        private const string IbanNumbersOnly = "1212341234123412341234";
        private TransactionService sut;
        private Mock<IMapper> mapper;
        private Mock<ITransactionRepository> transactionRepository;
        private Mock<IAccountDetailsRepository> accountDetailsRepository;

        [TestMethod]
        public async Task AddTransactionAsync_NotEnoughBalance_ThrowsException()
        {
            //Arrange
            var reciver = new Mock<IAccountDetail>();
            var sender = new Mock<IAccountDetail>();
            sender.Setup(_ => _.Balance).Returns(100);
            accountDetailsRepository.Setup(_ => _.GetByIdAsync(It.IsAny<int>(), null)).ReturnsAsync(sender.Object);
            accountDetailsRepository.Setup(_ => _.GetAccountByIbanAsync(It.IsAny<string>())).ReturnsAsync(reciver.Object);

            const int senderId = 1;
            var transactionVM = new NewTransactionViewModel()
            {
                ReciverIBAN = default_Iban,
                Amount = 123.12m,
                Description = "description",
                ReciverName = "Reciver Name",
                TransferDate = DateTime.Now,
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
            mapper = new Mock<IMapper>();
            transactionRepository = new Mock<ITransactionRepository>();
            accountDetailsRepository = new Mock<IAccountDetailsRepository>();
            sut = new TransactionService(transactionRepository.Object, accountDetailsRepository.Object, mapper.Object);
        }
    }
}