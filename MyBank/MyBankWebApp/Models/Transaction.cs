using System.ComponentModel.DataAnnotations;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public int Reciver { get; set; }

        //Sender is NULL for deposits
        public int? Sender { get; set; }
        
        public TransactionTypes TransactionTypes { get; set; }
        
        public TransactionStatus Status { get; set; }
        
        public DateTime CreationTime {  get; set; }
    }
}