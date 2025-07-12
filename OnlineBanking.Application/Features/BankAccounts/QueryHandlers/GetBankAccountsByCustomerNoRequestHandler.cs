
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Features.BankAccounts.Queries;
using OnlineBanking.Application.Features.Customers;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.Application.Features.BankAccounts.QueryHandlers;

public class GetBankAccountsByCustomerNoRequestHandler : IRequestHandler<GetBankAccountsByCustomerNoRequest, ApiResult<PagedList<BankAccountResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IBankAccountMapper _bankAccountMapper;

    public GetBankAccountsByCustomerNoRequestHandler(IUnitOfWork uow,
                                                    IBankAccountMapper bankAccountMapper)
    {
        _uow = uow;
        _bankAccountMapper = bankAccountMapper;
    }

    public async Task<ApiResult<PagedList<BankAccountResponse>>> Handle(GetBankAccountsByCustomerNoRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<BankAccountResponse>>();

        if (!await _uow.Customers.ExistsAsync(request.CustomerNo))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(CustomerErrorMessages.NotFound, "No.", request.CustomerNo));

            return result;
        }
        
        var bankAccountParams = request.BankAccountParams;
        var accountTransactionsParams = request.AccountTransactionsParams;
        var customerBankAccounts = new List<BankAccountResponse>();

        var (bankAccounts, totalCount) = await _uow.BankAccounts.GetBankAccountsByCustomerNoAsync(request.CustomerNo, bankAccountParams);

        foreach (var bankAccount in bankAccounts)
        {
            var bankAccountOwners = await _uow.Customers.GetByIBANAsync(bankAccount.IBAN);
            var (cashTransactions, transactionsCount) = await _uow.CashTransactions.GetByIBANAsync(bankAccount.IBAN, accountTransactionsParams);

            var cashTransactionsPagedList = cashTransactions.ToPagedList(transactionsCount, 
                                                                        accountTransactionsParams.PageNumber, 
                                                                        accountTransactionsParams.PageSize, 
                                                                        cancellationToken);

            var customerBankAccount = _bankAccountMapper.MapToResponseModel(bankAccount, bankAccountOwners, cashTransactionsPagedList);

            customerBankAccounts.Add(customerBankAccount);
        }

        result.Payload = customerBankAccounts.ToPagedList(customerBankAccounts.Count, 
                                                          bankAccountParams.PageNumber,
                                                          bankAccountParams.PageSize, 
                                                          cancellationToken);
        return result;
    }
}