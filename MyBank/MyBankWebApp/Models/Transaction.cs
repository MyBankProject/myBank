using MyBankWebApp.Models.Abstractions;
using System.ComponentModel.DataAnnotations;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreationTime { get; set; }

        public string? Description { get; set; }

        public int Reciver { get; set; }

        public int Sender { get; set; }

        public TransactionStatus Status { get; set; }
        
        public TransactionTypes TransactionType { get; set; }

        public IAccountDetail? ReciverAccountDetails { get; set; }

        public IAccountDetail? SenderAccountDetails { get; set; }
    }
}