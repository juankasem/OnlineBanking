using OnlineBanking.Application.Features.BankAccounts.Queries;

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
        var (cashTransactions, totalCount) = await _uow.CashTransactions.GetByAccountNoOrIBANAsync(bankAccount.IBAN, request.AccountTransactionsParams);


        result.Payload = _bankAccountMapper.MapToResponseModel(bankAccount, bankAccountOwners, cashTransactions);

        return result;
    }
}
