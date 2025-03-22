using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBankWebApp.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(34)")]
        [MaxLength(34)]
        public required string IBAN { get; set; }

        [Required]
        [Column(TypeName = "varchar(2)")]
        [MaxLength(2)]
        public required string CountryCode { get; set; }

        public decimal Balance { get; set; } = 0;

        public ICollection<Transaction>? SentTransactions { get; set; }

        public ICollection<Transaction>? ReceivedTransactions { get; set; }
    }
}
