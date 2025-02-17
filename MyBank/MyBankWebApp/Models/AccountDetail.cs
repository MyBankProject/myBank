using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.Models
{
    public class AccountDetail
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public required string IBAN { get; set; }

        [Required]
        public required string IBAN_Prefix { get; set; }

        public decimal Balance { get; set; } = 0;

        public ICollection<Transaction>? Transfers { get; set; }
    }
}
