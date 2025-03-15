using AutoMapper;
using MyBankWebApp.DTOs;
using MyBankWebApp.Models;

namespace MyBankWebApp.Mappers
{
    public class NewTransactionVMToTransactionMapper : Profile
    {
        public NewTransactionVMToTransactionMapper()
        {
            CreateMap<NewTransactionViewModel, Transaction>()
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.TransferDate))
                .ForMember(dest => dest.Sender, opt => opt.MapFrom(src => src.SenderId));
        }
    }
}
