using AutoMapper;
using MyBankWebApp.Models;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Mappers
{
    public class NewTransactionToTransactionMap : Profile
    {
        public NewTransactionToTransactionMap()
        {
            CreateMap<NewTransactionViewModel, Transaction>()
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.TransferDate));
        }
    }
}
