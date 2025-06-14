using MyBankWebApp.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreationTime { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }
        public required int StatusId {  get; set; }
        public TransactionStatus? Status { get; set; }
        public required int TransactionTypeId {  get; set; }
        public TransactionType? TransactionType { get; set; }
        public Account? ReceiverAccount { get; set; }
        public int ReceiverId { get; set; }
        public Account? SenderAccount { get; set; }
        public int SenderId { get; set; }
    }
}