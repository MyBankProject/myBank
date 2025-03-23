namespace MyBankWebApp.Exceptions
{
    public class UserNotFoundException(string message) : Exception(message)
    {
    }
}
