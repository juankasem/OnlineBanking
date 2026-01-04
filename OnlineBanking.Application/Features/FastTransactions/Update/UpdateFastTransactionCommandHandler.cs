
using OnlineBanking.Application.Helpers;

namespace OnlineBanking.Application.Features.FastTransactions.Update;

public class UpdateFastTransactionCommandHandler(
    IUnitOfWork uow,
    IBankAccountHelper bankAccountHelper,
    ILogger<UpdateFastTransactionCommandHandler> logger) : IRequestHandler<UpdateFastTransactionCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountHelper _bankAccountHelper = bankAccountHelper;
    private readonly ILogger<UpdateFastTransactionCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(UpdateFastTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start updating fast transaction of bankac account IBAn {iban}", request.IBAN);

        var result = new ApiResult<Unit>();
        
        var senderIBAN = request.IBAN;
        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(senderIBAN);

        if (!_bankAccountHelper.ValidateBankAccount(bankAccount, senderIBAN, result))
            return result;

        var recipientIBAN = request.RecipientIBAN;
        var recipientBankAccount = await _uow.BankAccounts.GetByIBANAsync(recipientIBAN);

        if (!_bankAccountHelper.ValidateBankAccount(recipientBankAccount, recipientIBAN, result))
            return result;

        var fastTransaction = FastTransaction.Create(bankAccount.Id, 
                                                     request.RecipientIBAN,
                                                     request.RecipientName, 
                                                     request.Amount, 
                                                     request.Id);

        //Add transaction to sender's account
        bankAccount.UpdateFastTransaction(request.Id, fastTransaction);

        //Update sender's account
        await _uow.SaveAsync();
        
        return result;
    }
}
