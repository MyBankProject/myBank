using Microsoft.AspNetCore.Identity;
using MyBankWebApp.Data;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Entities;
using MyBankWebApp.Services.Abstractions;

namespace MyBankWebApp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IPasswordHasher<User> passwordHasher;

        public UserService(ApplicationDbContext dbContext, IPasswordHasher<User> passwordHasher)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
        }

        public void RegisterUser(RegisterUserDto dto)
        {
            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                RoleId = dto.RoleId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };
            newUser.PasswordHash =  passwordHasher.HashPassword(newUser, dto.Password);
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
        }
    }
}
