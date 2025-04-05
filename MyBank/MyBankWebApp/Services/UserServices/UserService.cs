using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBankWebApp.Data;
using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Entities;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Services.Accounts.Abstractions;
using MyBankWebApp.Services.UserServices.Abstractions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyBankWebApp.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IAccountService accountService;
        private readonly AuthenticationSettings authenticationSettings;
        private readonly ApplicationDbContext dbContext;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IValidator<RegisterUserDto> validator;
        private readonly ILogger<UserService> logger;

        public UserService(
            ApplicationDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            AuthenticationSettings authenticationSettings,
            IValidator<RegisterUserDto> validator,
            IAccountService accountService)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
            this.authenticationSettings = authenticationSettings;
            this.validator = validator;
            this.accountService = accountService;
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

        public async Task<User> GetUserAsync(int id, Func<IQueryable<User>, IQueryable<User>>? include = null)
        {
            IQueryable<User> query = dbContext.Users;
            if (include != null)
            {
                query = include(query);
            }
            User? user = await query.FirstOrDefaultAsync(user => user.Id == id);
            return user ?? throw new InvalidUserIdException($"Cound not find user with id {id}");
        }

        public async Task<List<string>> RegisterUser(RegisterUserDto dto)
        {
            var result = validator.Validate(dto);
            if (!result.IsValid)
            {
                return result.Errors.Select(e => e.ErrorMessage).ToList();
            }

            var transaction = dbContext.Database.BeginTransaction();
            try
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
                newUser.PasswordHash = passwordHasher.HashPassword(newUser, dto.Password);
                Account newAccount = await accountService.CreateAccount(dto.Nationality);
                newUser.AccountId = newAccount.Id;
                dbContext.Users.Add(newUser);
                await dbContext.SaveChangesAsync();
                transaction.Commit();
                return null;
            }
            catch (Exception ex)
            {
                dbContext.Database.RollbackTransaction();
                logger.LogError(ex, ex.Message);
                return ["An error occurred while registering the user."];
            }
        }
    }
}