using AutoMapper;
using MyBankWebApp.Models;
using MyBankWebApp.ViewModels;

namespace MyBankWebApp.Mappers
{
    public class DepositVMToTransactionMap : Profile
    {
        public DepositVMToTransactionMap()
        {
            CreateMap<DepositViewModel, Transaction>()
                .ForMember(dest => dest.CreationTime, opt => opt.MapFrom(src => src.TransferDate));
        }
    }
}
