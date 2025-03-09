using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;

namespace MyBankWebApp.Services.UserServices.Abstractions
{
    public interface IUserService
    {
        List<string> RegisterUser(RegisterUserDto dto);
        string GenerateJwt(LoginDto dto);
    }
}
