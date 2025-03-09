using AutoMapper;
using MyBankWebApp.DTOs;
using MyBankWebApp.Models;

namespace MyBankWebApp.Mappers
{
    public class NewTransactionDtoToTransactionMapper : Profile
    {
        public NewTransactionDtoToTransactionMapper()
        {
            CreateMap<NewTransactionDto, Transaction>()
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.TransferDate))
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.SenderId));
        }
    }
}
