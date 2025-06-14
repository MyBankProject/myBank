namespace MyBankWebApp.ViewModels
{
    public class AccountViewModel
    {
        public int Id { get; set; }
        public decimal Balance { get; set; } = 0;
        public required string CountryCode { get; set; }
        public required string Iban { get; set; }
        public List<TransactionViewModel>? Transactions { get; set; }
    }
}