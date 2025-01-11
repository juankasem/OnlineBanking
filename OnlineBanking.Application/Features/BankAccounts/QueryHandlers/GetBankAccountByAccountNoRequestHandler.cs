using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts.Queries;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.Application.Features.BankAccounts.QueryHandlers;

public class GetBankAccountByAccountNoRequestHandler : IRequestHandler<GetBankAccountByAccountNoRequest, ApiResult<BankAccountResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IBankAccountMapper _bankAccountMapper;

    public GetBankAccountByAccountNoRequestHandler(IUnitOfWork uow, IBankAccountMapper bankAccountMapper)
    {
        _uow = uow;
        _bankAccountMapper = bankAccountMapper;
    }

    public async Task<ApiResult<BankAccountResponse>> Handle(GetBankAccountByAccountNoRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<BankAccountResponse>();

        var bankAccount = await _uow.BankAccounts.GetByAccountNoAsync(request.AccountNo);

        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "No.", request.AccountNo));

            return result;
        }

        var bankAccountOwners = await _uow.Customers.GetByIBANAsync(bankAccount.IBAN);
        var accountCashTransactions = await _uow.CashTransactions.GetByIBANAsync(bankAccount.IBAN, request.AccountTransactionsParams);

        result.Payload = _bankAccountMapper.MapToResponseModel(bankAccount, bankAccountOwners, accountCashTransactions);

        return result;
    }
}
