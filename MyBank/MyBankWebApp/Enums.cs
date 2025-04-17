namespace MyBankWebApp
{
    public class Enums
    {
        public enum TransactionDirections
        {
            Incoming,
            Outgoing
        }

        public enum TransactionStatuses
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

        public enum Roles
        {
            User,
            Menager,
            Admin
        }
    }
}