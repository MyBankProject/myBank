﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using MyBankWebApp.Data;
using MyBankWebApp.DTOs;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.Exceptions;
using MyBankWebApp.Models;
using MyBankWebApp.Models.Users;
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
        private readonly ILogger<UserService> logger;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IValidator<RegisterUserDto> validator;

        public UserService(
            ApplicationDbContext dbContext,
            IPasswordHasher<User> passwordHasher,
            AuthenticationSettings authenticationSettings,
            IValidator<RegisterUserDto> validator,
            IAccountService accountService,
            ILogger<UserService> logger)
        {
            this.dbContext = dbContext;
            this.passwordHasher = passwordHasher;
            this.authenticationSettings = authenticationSettings;
            this.validator = validator;
            this.accountService = accountService;
            this.logger = logger;
        }

        public async Task<bool> AnyUserByQueryAsync(Func<IQueryable<User>, IQueryable<User>> query)
        {
            if (query == null)
            {
                return false;
            }
            IQueryable<User> queryToCheck = query(dbContext.Users);
            return await queryToCheck.AnyAsync();
        }

        public string GenerateJwt(LoginDto dto)
        {
            User user = dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Email == dto.Email) 
                ?? throw new BadReQuestException("Invalid username or password");
            PasswordVerificationResult result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadReQuestException("Invalid username or password");
            }

            var claims = new List<Claim>()
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Role, $"{user.Role.Name}"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            DateTime expires = DateTime.Now.AddDays(authenticationSettings.JwtExpireDays);

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
            return user ?? throw new InvalidIdException($"Could not find user with id {id}");
        }

        public async Task<User> GetUserByStringIdAsync(string? stringId, Func<IQueryable<User>, IQueryable<User>>? include = null)
        {
            if (!int.TryParse(stringId, out int id))
            {
                throw new InvalidIdException($"Could not get user Id {id}");
            }
            return await GetUserAsync(id, include);
        }

        public async Task<List<string>> RegisterUser(RegisterUserDto dto)
        {
            ValidationResult result = validator.Validate(dto);
            if (!result.IsValid)
            {
                return [.. result.Errors.Select(e => e.ErrorMessage)];
            }

            IDbContextTransaction transaction = await dbContext.Database.BeginTransactionAsync();
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
                await transaction.CommitAsync();
                return null;
            }
            catch (Exception ex)
            {
                await dbContext.Database.RollbackTransactionAsync();
                logger.LogError(ex, ex.Message);
                return ["An error occurred while registering the user."];
            }
        }
    }
}