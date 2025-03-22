using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.Models.Enums
{
    public class TransactionStatus
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public required string Name { get; set; }
    }
}
