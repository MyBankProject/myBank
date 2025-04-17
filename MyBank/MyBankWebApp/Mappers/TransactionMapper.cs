using AutoMapper;
using MyBankWebApp.Models;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Mappers
{
    public class TransactionMapper : Profile
    {
        public TransactionMapper()
        {
            CreateMap<NewTransactionViewModel, Transaction>()
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.TransferDate))
                .ForMember(dest => dest.SenderId, opt => opt.MapFrom(src => src.SenderId));
        }
    }
}
