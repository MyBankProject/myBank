using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.ViewModels
{
    public class UserViewModel
    {
        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email fromat")]
        public required string Email { get; set; }

        [MinLength(6)]
        public required string Password { get; set; }
        
        [MinLength(6)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }
        
        public required string Nationality { get; set; }

        [Required(ErrorMessage = "Date of Birth is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date in the format dd.mm.yyyy.")]
        public required DateTime DateOfBirth { get; set; }
    }
}
