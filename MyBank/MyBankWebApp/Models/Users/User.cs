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
        public required string Email { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        public required string FirstName { get; set; }

        [Required]
        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        public required string LastName { get; set; }

        [Required]
        [Column(TypeName = "varchar(50)")]
        [MaxLength(50)]
        public required string Nationality { get; set; }
        public string PasswordHash { get; set; } = "";
        public DateTime DateOfBirth { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = new Role();
        public int? AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
