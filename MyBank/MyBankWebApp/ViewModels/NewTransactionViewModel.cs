using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.ViewModels
{
    public class NewTransactionViewModel
    {
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string ReceiverIBAN { get; set; } = string.Empty;

        [Required]
        public string ReceiverName { get; set; } = string.Empty;

        public int SenderId { get; set; }

        public DateTime TransferDate { get; set; } = DateTime.Now;
    }
}