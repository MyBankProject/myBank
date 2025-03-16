namespace MyBankWebApp.ViewModels
{
    public class AccountDetailViewModel
    {
        public decimal Balance { get; set; } = 0;
        public required string CountryCode { get; set; }
        public required string IBAN { get; set; }
        public List<TransactionViewModel>? Transactions { get; set; }
        public int UserId { get; set; }
    }
}