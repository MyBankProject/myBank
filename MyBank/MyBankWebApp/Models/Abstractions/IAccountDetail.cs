namespace MyBankWebApp.Models.Abstractions
{
    public interface IAccountDetail
    {
        decimal Balance { get; set; }
        string CountryCode { get; set; }
        string IBAN { get; set; }
        ICollection<Transaction>? RecivedTransactions { get; set; }
        ICollection<Transaction>? SentTransactions { get; set; }
        int UserId { get; set; }
    }
}