using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Entities;

namespace MyBankWebApp.Services.UserServices.Abstractions
{
    public interface IUserService
    {
        string GenerateJwt(LoginDto dto);
        Task<User> GetUserAsync(int id, Func<IQueryable<User>, IQueryable<User>>? include = null);
        Task<List<string>> RegisterUser(RegisterUserDto dto);
    }
}