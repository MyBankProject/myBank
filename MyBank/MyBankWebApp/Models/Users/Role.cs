using System.ComponentModel.DataAnnotations.Schema;

namespace MyBankWebApp.Models.Users
{
    public class Role
    {
        public int Id { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; } = "";
    }
}
