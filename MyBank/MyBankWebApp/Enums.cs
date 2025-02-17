namespace MyBankWebApp
{
    public class Enums
    {
        public enum TransactionTypes
        {
            deposit,
            transfer
        };

        public enum TransactionStatus
        {
            pending,
            completed,
            failed
        }
    }
}
