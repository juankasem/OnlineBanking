
using OnlineBanking.Application.Features.Branch.Create;
using OnlineBanking.Application.Features.Branch.Update;
using OnlineBanking.Application.Features.Customers.Create;
using OnlineBanking.Application.Features.FastTransactions.Update;
using OnlineBanking.Application.Models.Address;
using OnlineBanking.Application.Models.Auth.Responses;
using OnlineBanking.Application.Models.BankAccount.Requests;
using OnlineBanking.Application.Models.Branch;
using OnlineBanking.Application.Models.Branch.Requests;
using OnlineBanking.Application.Models.Branch.Responses;
using OnlineBanking.Application.Models.CashTransaction.Requests;
using OnlineBanking.Application.Models.Customer.Requests;
using OnlineBanking.Application.Models.Customer.Responses;
using OnlineBanking.Application.Models.FastTransaction.Requests;
using OnlineBanking.Application.Models.FastTransaction.Responses;
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
        CreateMap<BankAccountDto, BankAccountResponse>().ReverseMap();
        CreateMap<BankAccount, BankAccountDto>()
            .ForPath(d => d.Branch.Id, o => o.MapFrom(s => s.Branch.Id))
            .ForPath(d => d.Branch.BranchName, o => o.MapFrom(s => s.Branch.Name))
            .ForPath(d => d.AccountBalance.Balance, o => o.MapFrom(s => s.Balance))
            .ForPath(d => d.AccountBalance.AllowedBalanceToUse, o => o.MapFrom(s => s.AllowedBalanceToUse))
            .ForPath(d => d.AccountBalance.MinimumAllowedBalance, o => o.MapFrom(s => s.MinimumAllowedBalance))
            .ForPath(d => d.AccountBalance.Debt, o => o.MapFrom(s => s.Debt))
            .ForPath(d => d.Currency.Id, o => o.MapFrom(s => s.Currency.Id))
            .ForPath(d => d.Currency.Code, o => o.MapFrom(s => s.Currency.Code))
            .ForPath(d => d.Currency.Name, o => o.MapFrom(s => s.Currency.Name))
            .ForPath(d => d.Currency.Symbol, o => o.MapFrom(s => s.Currency.Symbol))
            .ReverseMap();

        CreateMap<AppUser, AuthResponse>().ReverseMap();

        //Branches
        CreateMap<Branch, BranchResponse>()
            .ForMember(d => d.BranchId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.BranchName, o => o.MapFrom(s => s.Name))
            .ForMember(d => d.BranchAddress, o => o.MapFrom(s => s.Address))
            .ReverseMap();

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
        .ForMember(d => d.IBAN, o => o.MapFrom(s => s.BankAccount.IBAN))
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