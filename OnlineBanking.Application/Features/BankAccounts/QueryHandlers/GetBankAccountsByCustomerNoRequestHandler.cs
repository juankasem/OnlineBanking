using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts.Queries;
using OnlineBanking.Application.Features.Customers;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;
using OnlineBanking.Core.Helpers;

namespace OnlineBanking.Application.Features.BankAccount.QueryHandlers;

public class GetBankAccountsByCustomerNoRequestHandler : IRequestHandler<GetBankAccountsByCustomerNoRequest, ApiResult<PagedList<BankAccountResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IBankAccountMapper _bankAccountMapper;

    public GetBankAccountsByCustomerNoRequestHandler(IUnitOfWork uow,
                                                    IMapper mapper,
                                                    IBankAccountMapper bankAccountMapper)
    {
        _uow = uow;
        _mapper = mapper;
        _bankAccountMapper = bankAccountMapper;
    }

    public async Task<ApiResult<PagedList<BankAccountResponse>>> Handle(GetBankAccountsByCustomerNoRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<BankAccountResponse>>();

        if (!await _uow.Customers.ExistsAsync(request.CustomerNo))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(CustomerErrorMessages.NotFound, "Customer No.", request.CustomerNo));

            return result;
        }
        
        var reqParams = request.BankAccountParams;
        var customerBankAccouns = new List<BankAccountResponse>();

        var bankAccounts = await _uow.BankAccounts.GetAccountsByCustomerNoAsync(request.CustomerNo);

        foreach (var bankAccount in bankAccounts)
        {
            var accountTransactions = await _uow.CashTransactions.GetByIBANAsync(bankAccount.IBAN, request.AccountTransactionsParams);
            var customerBankAccount = _bankAccountMapper.MapToResponseModel(bankAccount, accountTransactions);

            customerBankAccouns.Add(customerBankAccount);
        }

        result.Payload = PagedList<BankAccountResponse>.Create(customerBankAccouns, reqParams.PageNumber, reqParams.PageSize);

        return result;
    }
}