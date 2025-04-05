using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;

namespace MyBankWebApp.Services.UserServices.Abstractions
{
    public interface IUserService
    {
        string GenerateJwt(LoginDto dto);
        Task<List<string>> RegisterUser(RegisterUserDto dto);
    }
}