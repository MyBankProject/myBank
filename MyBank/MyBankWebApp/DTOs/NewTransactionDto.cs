using System.ComponentModel.DataAnnotations;

namespace MyBankWebApp.DTOs
{
    public class NewTransactionDto
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ReciverIBAN { get; set; }

        [Required]
        public int ReciverName { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        //TODO: dodać walidację żeby nie dało się wysłać przelewu z przeszłości
        public DateTime TransferDate { get; set; }
    }
}