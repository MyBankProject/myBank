using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBankWebApp;
using MyBankWebApp.Data;
using MyBankWebApp.Middlewares;
using MyBankWebApp.Models.Users;
using MyBankWebApp.Models.Validators;
using MyBankWebApp.Repositories;
using MyBankWebApp.Repositories.Abstractions;
using MyBankWebApp.Services.Accounts;
using MyBankWebApp.Services.Accounts.Abstractions;
using MyBankWebApp.Services.Transactions;
using MyBankWebApp.Services.Transactions.Abstractions;
using MyBankWebApp.Services.UserServices;
using MyBankWebApp.Services.UserServices.Abstractions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllersWithViews();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>(); 

//Register Services
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();

//Register Mappers
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Register Db Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddSwaggerGen();

var authenticationSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
    };

    // Get token from cookie
    cfg.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("AuthToken"))
            {
                context.Token = context.Request.Cookies["AuthToken"];
            }
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            context.HandleResponse(); 
            context.Response.Redirect("/User/Login"); 
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddAuthorization();
var app = builder.Build();

//Setup default data to database
Seed.SeedData(app);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyBank Api");
});

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();