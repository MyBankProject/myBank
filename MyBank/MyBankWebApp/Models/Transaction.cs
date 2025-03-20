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

        //UStawić max length
        public string? Description { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionTypes TransactionType { get; set; }

        public Account? ReceiverAccount { get; set; }
        public int ReceiverId { get; set; }

        public Account? SenderAccount { get; set; }
        public int SenderId { get; set; }
    }
}