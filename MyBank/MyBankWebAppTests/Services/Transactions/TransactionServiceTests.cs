using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyBankWebApp.Data;
using MyBankWebApp.Services.Transactions;
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
        TransactionService sut;
        Mock<IMapper> mapper;

        [TestMethod]
        public void TransactionServiceTest()
        {
            Assert.Fail();
        }

        [TestInitialize]
        public void XInitialize()
        {
        }
    }
}