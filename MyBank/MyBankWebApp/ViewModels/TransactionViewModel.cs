using MyBankWebApp.Models;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.ViewModels
{
    public class TransactionViewModel
    {
        public decimal Amount { get; set; }
        public DateTime CreationTime { get; set; }
        public string? Description { get; set; }
        public int Id { get; set; }
        public int OtherSideOfTransaction { get; set; }
        public TransactionStatuses Status { get; set; }
        public TransactionDirections TransactionDirection { get; set; }
        public TransactionTypes TransactionType { get; set; }
    }
}