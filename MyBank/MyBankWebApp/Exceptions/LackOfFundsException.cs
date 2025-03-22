namespace MyBankWebApp.Exceptions
{
    public class LackOfFundsException(string message) : Exception(message)
    {
    }
}
