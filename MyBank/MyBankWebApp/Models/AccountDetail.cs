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

        public ICollection<Transaction>? SentTransactions {  get; set; }
        public ICollection<Transaction>? RecivedTransactions {  get; set; }

        //nie wiem czy dobrze zrobiłem rozdzielając sent i recive ale nie wiem czy da się to zrobić razem w bazie danych, żeby tabelki nie były zbyt
        //zagmatwane. Piszę tutaj żebym nie zapomniał przy PRze, daj znać co o tym myślisz.
        [NotMapped]
        public List<Transaction>? Transactions { get; set; }
    }
}
