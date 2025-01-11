using AutoMapper;
using OnlineBanking.Application.Features.BankAccounts.Commands;
using OnlineBanking.Application.Features.Branch.Commands;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Features.Customers.Commands;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Models.Address;
using OnlineBanking.Application.Models.Auth.Responses;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Application.Models.BankAccount.Requests;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.Branch.Requests;
using OnlineBanking.Application.Models.Branch.Responses;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.Customer.Requests;
using OnlineBanking.Application.Models.Customer.Responses;
using OnlineBanking.Application.Models.FastTransaction.Requests;
using OnlineBanking.Application.Models.FastTransaction.Responses;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.BranchAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Entities;

namespace OnlineBanking.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //bank Accounts
        CreateMap<CreateBankAccountRequest, CreateBankAccountCommand>();        
        CreateMap<BankAccountDto,BankAccountResponse>().ReverseMap();
        CreateMap<BankAccount, BankAccountDto>().ReverseMap();

        CreateMap<AppUser, AuthResponse>().ReverseMap();

        //Branches
        CreateMap<Branch, BranchResponse>()
            .ForMember(d => d.BranchId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.BranchAddress, o => o.MapFrom(s => s.Address)).ReverseMap();

        CreateMap<CreateBranchRequest, CreateBranchCommand>().ReverseMap();
        CreateMap<UpdateBranchRequest, UpdateBranchCommand>().ReverseMap();

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
        CreateMap<Core.Domain.Aggregates.AddressAggregate.Address, AddressDto>().ReverseMap();

        CreateMap<CreateCustomerRequest, CreateCustomerCommand>()
                .ForMember(d => d.BirthDate, o => o.MapFrom<DateTimeValueResolver>())
                .ReverseMap();
        CreateMap<Customer, CustomerResponse>().ReverseMap();
    }
}