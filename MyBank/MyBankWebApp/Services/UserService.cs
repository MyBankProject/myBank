using MyBankWebApp.Data;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Entities;
using MyBankWebApp.Services.Abstractions;

namespace MyBankWebApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                PasswordHash = dto.Password,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                RoleId = dto.RoleId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
        }
    }
}
