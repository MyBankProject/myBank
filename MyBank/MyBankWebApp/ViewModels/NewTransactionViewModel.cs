using Microsoft.OpenApi.MicrosoftExtensions;
using MyBankWebApp.Atributes;
using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.ViewModels
{
    public class NewTransactionViewModel
    {
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ReciverIBAN { get; set; }

        [Required]
        public string ReciverName { get; set; }

        public int SenderId { get; set; }

        [Required]
        [PastDate(ErrorMessage = "Transaction date cannot be in the past.")]
        public DateTime TransferDate { get; set; } = DateTime.Today;
    }
}