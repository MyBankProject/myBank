using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Entities;

namespace MyBankWebApp.Services.UserServices.Abstractions
{
    public interface IUserService
    {
        Task<bool> AnyUserByQuerryAsync(Func<IQueryable<User>, IQueryable<User>> query);

        string GenerateJwt(LoginDto dto);

        Task<User> GetUserByIdAsync(int id, Func<IQueryable<User>, IQueryable<User>>? include = null);

        Task RegisterUser(RegisterUserDto dto);
    }
}