using MyBankWebApp.DTOs.Creates;

namespace MyBankWebApp.Services.Abstractions
{
    public interface IUserService
    {
        void RegisterUser(RegisterUserDto dto);
    }
}
