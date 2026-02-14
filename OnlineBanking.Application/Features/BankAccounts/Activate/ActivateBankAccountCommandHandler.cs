
namespace OnlineBanking.Application.Features.BankAccounts.Activate;

/// <summary>
/// Handles requests to activate a bank account.
/// Validates the account exists and updates its activation status.
/// </summary>
public class ActivateBankAccountCommandHandler(
    IUnitOfWork uow,
    ILogger<ActivateBankAccountCommandHandler> logger)  : 
    IRequestHandler<ActivateBankAccountCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly ILogger<ActivateBankAccountCommandHandler> _logger = logger;

    /// <summary>
    /// Handles the request to activate a bank account.
    /// </summary>
    public async Task<ApiResult<Unit>> Handle(
        ActivateBankAccountCommand request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = new ApiResult<Unit>();
        var bankAccountId = request.BankAccountId;

        _logger.LogInformation(
            "Processing bank account activation: {BankAccountId}",
            bankAccountId);

        // Retrieve and validate bank account exists
        var bankAccount = await _uow.BankAccounts.GetByIdAsync(bankAccountId);
        if (bankAccount is null)
        {
            _logger.LogWarning(
                "Bank account validation failed: Account {BankAccountId} not found", 
                bankAccountId);

            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "Id", bankAccountId));
            return result;
        }

        // Activate the account
        bankAccount.Activate();

        // Persist changes
        _uow.BankAccounts.Update(bankAccount);

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation(
              "Bank account activation successful: {BankAccountId}",
              bankAccountId);
        }
        else
        {
            _logger.LogError(
                "Failed to persist bank account activation for account: {BankAccountId}. " +
                "Database transaction returned 0 rows affected",
                bankAccountId);
            result.AddError(ErrorCode.UnknownError, BankAccountErrorMessages.Unknown);
        }

        return result;
    }
}