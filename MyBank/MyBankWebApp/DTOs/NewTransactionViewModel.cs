using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.DTOs
{
    public class NewTransactionViewModel
    {
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public required decimal Amount { get; set; }

        public required string Description { get; set; }

        public required string ReciverIBAN { get; set; }

        public required string ReciverName { get; set; }

        public int SenderId { get; set; }

        [Required]
        //TODO: dodać walidację żeby nie dało się wysłać przelewu z przeszłości
        public DateTime TransferDate { get; set; }
    }
}