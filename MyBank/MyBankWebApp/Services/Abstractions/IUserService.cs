using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;

namespace MyBankWebApp.Services.Abstractions
{
    public interface IUserService
    {
        void RegisterUser(RegisterUserDto dto);
        string GenerateJwt(LoginDto dto);
    }
}
