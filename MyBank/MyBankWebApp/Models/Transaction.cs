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

        public AccountDetail? ReciverAccountDetails { get; set; }

        public AccountDetail? SenderAccountDetails { get; set; }
    }
}