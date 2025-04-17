using AutoMapper;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Mappers
{
    public class RegisterUserDtoMapper : Profile
    {
        public RegisterUserDtoMapper()
        {
            CreateMap<UserViewModel, RegisterUserDto>();
        }
    }
}
