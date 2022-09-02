using AutoMapper;
using OnlineBanking.Application.Features.BankAccounts.Commands;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Features.Customers.Commands;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Application.Models.BankAccount.Requests;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.Customer.Requests;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;

namespace OnlineBanking.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Accounts
        CreateMap<CreateBankAccountRequest,CreateBankAccountCommand>()
                .ForMember(d => d.AccountBalance.Balance, o => o.MapFrom(s => s.Balance))
                .ForMember(d => d.AccountBalance.AllowedBalanceToUse, o => o.MapFrom(s => s.AllowedBalanceToUse))
                .ForMember(d => d.AccountBalance.MinimumAllowedBalance, o => o.MapFrom(s => s.MinimumAllowedBalance))
                .ForMember(d => d.AccountBalance.Debt, o => o.MapFrom(s => s.Debt));
        

        CreateMap<BankAccountDto,BankAccountResponse>().ReverseMap();

        //Cash Transactions
        CreateMap<CreateCashTransactionRequest, MakeDepositCommand>().ReverseMap();
        CreateMap<CreateCashTransactionRequest, MakeFundsTransferCommand>().ReverseMap();
        CreateMap<CreateCashTransactionRequest, MakeWithdrawalCommand>().ReverseMap();

        //Customers
        CreateMap<CreateCustomerRequest, CreateCustomerCommand>();
    }
}