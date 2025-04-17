using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBankWebApp.Models.Enums
{
    public class TransactionType
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public required string Name { get; set; }
    }
}
