using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBankWebApp.Models
{
    public class AccountDetail
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(34)")]
        [MaxLength(34)]
        public required string IBAN { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(2)")]
        [MaxLength(2)]
        public required string CountryCode { get; set; }

        public decimal Balance { get; set; } = 0;

        public ICollection<Transaction>? Transfers { get; set; }
    }
}
