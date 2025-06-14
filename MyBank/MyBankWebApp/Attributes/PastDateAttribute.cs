using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date && date < DateTime.Today)
            {
                return new ValidationResult(ErrorMessage ?? "The date must be today or in the future");
            }
            return ValidationResult.Success;
        }
    }
}
