using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts.Queries;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.Application.Features.BankAccounts.QueryHandlers;

public class GetBankAccountWithTransactionsRequestHandler : IRequestHandler<GetBankAccountWithTransactionsRequest,ApiResult<BankAccountResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IBankAccountMapper _bankAccountMapper;

    public GetBankAccountWithTransactionsRequestHandler(IUnitOfWork uow, IBankAccountMapper bankAccountMapper)
    {
        _uow = uow;
        _bankAccountMapper = bankAccountMapper;
    }

    public async Task<ApiResult<BankAccountResponse>> Handle(GetBankAccountWithTransactionsRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<BankAccountResponse>();

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.IBAN);

        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.IBAN));

            return result;
        }

        var bankAccountOwners = await _uow.Customers.GetByIBANAsync(bankAccount.IBAN);
        var accountTransactions = await _uow.CashTransactions.GetByIBANAsync(bankAccount.IBAN, request.AccountTransactionsParams);

        result.Payload = _bankAccountMapper.MapToResponseModel(bankAccount, bankAccountOwners, accountTransactions);

        return result;
    }
}