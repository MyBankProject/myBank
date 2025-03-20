using AutoMapper;
using MyBankWebApp.Models;
using MyBankWebApp.ViewModels;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Mappers
{
    public class AccountDetailsMapper : Profile
    {
        public AccountDetailsMapper()
        {
            CreateMap<Account, AccountDetailViewModel>()
                .ForMember(dest => dest.Transactions, opt =>
                    opt.MapFrom(src => src.SentTransactions.Concat(src.ReceivedTransactions).Select(t => new TransactionViewModel
                    {
                        Amount = t.Amount,
                        CreationTime = t.CreationTime,
                        Description = t.Description,
                        Id = t.Id,
                        OtherSideOfTransaction = t.ReceiverId == src.Id ? t.SenderId : t.ReceiverId,
                        TransactionDirection = t.ReceiverId == src.Id ? TransactionDirections.Incoming : TransactionDirections.Outgoing,
                        Status = t.Status,
                        TransactionType = t.TransactionType
                    })));
        }
    }
}