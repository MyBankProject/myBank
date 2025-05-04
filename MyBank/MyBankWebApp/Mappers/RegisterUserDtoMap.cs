using AutoMapper;
using MyBankWebApp.DTOs.Creates;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Mappers
{
    public class RegisterUserDtoMap : Profile
    {
        public RegisterUserDtoMap()
        {
            CreateMap<UserViewModel, RegisterUserDto>();
        }
    }
}
