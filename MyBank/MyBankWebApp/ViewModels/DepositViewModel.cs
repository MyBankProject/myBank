using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.ViewModels
{
    public class DepositViewModel
    {
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "The deposit value must be greater than zero")]
        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        public string ReceiverIBAN { get; set; } = string.Empty;

        public DateTime TransferDate { get; set; } = DateTime.Today;
    }
}