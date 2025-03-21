using AutoMapper;
using MyBankWebApp.Models;
using MyBankWebApp.ViewModels;
using static MyBankWebApp.Enums;

namespace MyBankWebApp.Mappers
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<Account, AccountViewModel>()
                .ForMember(dest => dest.Transactions, opt =>
                    opt.MapFrom(src => (src.SentTransactions ?? Enumerable.Empty<Transaction>())
                        .Concat(src.ReceivedTransactions ?? Enumerable.Empty<Transaction>())
                        .OrderBy(t => t.CreationTime)
                        .Select(t => new TransactionViewModel
                        {
                            Amount = (t.ReceiverId == src.Id ? t.Amount : - t.Amount),
                            CreationTime = t.CreationTime,
                            Description = t.Description,
                            Id = t.Id,
                            OtherSideOfTransaction = t.ReceiverId == src.Id ? t.SenderId : t.ReceiverId,
                            TransactionDirection = t.ReceiverId == src.Id ? TransactionDirections.Incoming : TransactionDirections.Outgoing,
                            Status = Enum.IsDefined(typeof(TransactionStatuses), t.StatusId) ? (TransactionStatuses)t.StatusId : default,
                        })));
        }
    }
}
