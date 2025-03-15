namespace MyBankWebApp.DTOs
{
    public class AccountDetailDto
    {
        public decimal Balance { get; set; } = 0;
        public required string CountryCode { get; set; }
        public required string IBAN { get; set; }
        public List<TransactionDto>? Transactions { get; set; }
        public int UserId { get; set; }
    }
}