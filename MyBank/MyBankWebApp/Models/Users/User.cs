using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyBankWebApp.Models.Users
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(256)")]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50)]
        public string Nationality { get; set; }
        public string PasswordHash { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public int? AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
