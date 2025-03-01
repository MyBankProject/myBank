using AutoMapper;
using MyBankWebApp.DTOs;
using MyBankWebApp.Models;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Mappers
{
    public class AccountDetailsMapper : Profile
    {
        public AccountDetailsMapper()
        {
            CreateMap<AccountDetail, AccountDetailDto>()
                .ForMember(dest => dest.Transactions, opt =>
                    opt.MapFrom(src => src.SentTransactions.Concat(src.RecivedTransactions).Select(t => new TransactionDto
                    {
                        Amount = t.Amount,
                        CreationTime = t.CreationTime,
                        Description = t.Description,
                        Id = t.Id,
                        OtherSideOfTransaction = t.Reciver == src.UserId ? t.Sender : t.Reciver,
                        TransactionDirection = t.Reciver == src.UserId ? TransactionDirections.Incoming : TransactionDirections.Outgoing,
                        Status = t.Status,
                        TransactionType = t.TransactionType
                    })));
        }
    }
}