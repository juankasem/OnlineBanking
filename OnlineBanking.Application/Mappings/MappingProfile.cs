using AutoMapper;
using OnlineBanking.Application.Features.BankAccounts.Commands;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Features.Customers.Commands;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Models.Address;
using OnlineBanking.Application.Models.Auth.Responses;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Application.Models.BankAccount.Requests;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.Branch.Responses;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Application.Models.CreditCard.Responses;
using OnlineBanking.Application.Models.Customer.Requests;
using OnlineBanking.Application.Models.FastTransaction.Requests;
using OnlineBanking.Application.Models.FastTransaction.Responses;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Core.Domain.Entities;

namespace OnlineBanking.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Accounts
        CreateMap<CreateBankAccountRequest, CreateBankAccountCommand>()
                .ForMember(d => d.AccountBalance.Balance, o => o.MapFrom(s => s.Balance))
                .ForMember(d => d.AccountBalance.AllowedBalanceToUse, o => o.MapFrom(s => s.AllowedBalanceToUse))
                .ForMember(d => d.AccountBalance.MinimumAllowedBalance, o => o.MapFrom(s => s.MinimumAllowedBalance))
                .ForMember(d => d.AccountBalance.Debt, o => o.MapFrom(s => s.Debt)).ReverseMap();
        

        CreateMap<BankAccountDto,BankAccountResponse>().ReverseMap();

        CreateMap<AppUser, AuthResponse>().ReverseMap();

        //Branches
            CreateMap<Branch, BranchResponse>()
                .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.BranchAddress, o => o.MapFrom(s => s.Address));
                
            CreateMap<Address, BranchAddressDto>().ReverseMap();

        //Cash Transactions
        CreateMap<CreateCashTransactionRequest, MakeDepositCommand>().ReverseMap();
        CreateMap<CreateCashTransactionRequest, MakeFundsTransferCommand>().ReverseMap();
        CreateMap<CreateCashTransactionRequest, MakeWithdrawalCommand>().ReverseMap();


        //Fast Transactions
        CreateMap<CreateFastTransactionRequest, CreateFastTransactionCommand>().ReverseMap();
        CreateMap<UpdateFastTransactionRequest, UpdateFastTransactionCommand>().ReverseMap();
        CreateMap<DeleteFastTransactionRequest, DeleteFastTransactionCommand>().ReverseMap();
        CreateMap<FastTransaction, FastTransactionResponse>()
        .ForMember(d => d.RecipientBankName, o => o.MapFrom(s => s.BankAccount.Branch.Name))
        .ReverseMap();

        //Customers
        CreateMap<Address, AddressDto>();
        CreateMap<CreateCustomerRequest, CreateCustomerCommand>();
    }
}