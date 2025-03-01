namespace MyBankWebApp.DTOs
{
    public class AccountDetailDto
    {
        public decimal Balance { get; set; } = 0;
        public string CountryCode { get; set; }
        public string IBAN { get; set; }
        public int UserId { get; set; }

        public List<TransactionDto>? Transactions { get; set; }
    }
}