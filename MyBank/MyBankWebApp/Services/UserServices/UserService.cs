using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBankWebApp.Data;
using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Entities;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Services.UserServices.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyBankWebApp.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly AuthenticationSettings authenticationSettings;
        private readonly ApplicationDbContext dbContext;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IValidator<RegisterUserDto> validator;

        public UserService(
            ApplicationDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            AuthenticationSettings authenticationSettings,
            IValidator<RegisterUserDto> validator)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
            this.authenticationSettings = authenticationSettings;
            this.validator = validator;
        }

        public string GenerateJwt(LoginDto dto)
        {
            var user = dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == dto.Email);
            if (user == null)
            {
                throw new BadReQuestException("Invalid username or password"); // trzeba bedzie zrobic middleware ktory bedzie lapal wyjatki
            }

            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadReQuestException("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(
                authenticationSettings.JwtIssuer,
                authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: credential);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public List<string> RegisterUser(RegisterUserDto dto)
        {
            var result = validator.Validate(dto);
            if (!result.IsValid)
            {
                return result.Errors.Select(e => e.ErrorMessage).ToList();
            }

            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Nationality = dto.Nationality,
                RoleId = dto.RoleId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
            };
            newUser.PasswordHash = passwordHasher.HashPassword(newUser, dto.Password);
            dbContext.Users.Add(newUser);
            dbContext.SaveChanges();
            return null;
        }
    }
}