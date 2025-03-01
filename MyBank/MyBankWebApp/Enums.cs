namespace MyBankWebApp
{
    public class Enums
    {
        public enum TransactionDirections
        {
            Incoming,
            Outgoing
        }

        public enum TransactionStatus
        {
            Pending,
            Completed,
            Failed
        }

        public enum TransactionTypes
        {
            Deposit,
            Transfer
        };
    }
}