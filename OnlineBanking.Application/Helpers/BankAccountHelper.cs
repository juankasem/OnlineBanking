using OnlineBanking.Application.Features.CashTransactions;

namespace OnlineBanking.Application.Helpers;

public interface IBankAccountHelper
{
    bool ValidateBankAccount(
       BankAccount? bankAccount,
       string iban,
       ApiResult<Unit> result);
    bool HasSufficientFunds(
        BankAccount? senderAccount,
        decimal totalAmount,
        ApiResult<Unit> result);
}

/// <summary>
/// Helper class for bank account validation operations.
/// Provides centralized validation logic for account existence, status, and fund availability.
/// </summary>
public class BankAccountHelper(ILogger<BankAccountHelper> logger) : IBankAccountHelper
{
    private readonly ILogger<BankAccountHelper> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Validates that a bank account exists, is active, and meets basic requirements.
    /// </summary>
    /// <param name="bankAccount">The bank account to validate</param>
    /// <param name="iban">The IBAN identifier for logging and error messages</param>
    /// <param name="result">The API result object to populate with errors</param>
    /// <param name="logger">Optional logger for diagnostic information</param>
    /// <returns>True if the account is valid; otherwise false</returns>
    public bool ValidateBankAccount(
       BankAccount? bankAccount, 
       string iban,
       ApiResult<Unit> result)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(iban);
        ArgumentNullException.ThrowIfNull(result);

        // Account existence check
        if (bankAccount == null)
        {
            _logger.LogWarning(
              "Bank account validation failed: Account not found for IBAN {IBAN}",
              iban);

            result.AddError(ErrorCode.BadRequest, 
                string.Format(BankAccountErrorMessages.NotFound, "IBAN.", iban));
            return false;
        }

        // Account active status check
        if (!bankAccount.IsActive)
        {
            _logger.LogWarning(
                "Bank account validation failed: Account is inactive for IBAN {IBAN}", 
                iban);

            result.AddError(ErrorCode.BadRequest, 
                string.Format(BankAccountErrorMessages.Inactive, iban));
            return false;
        }

        _logger.LogDebug(
            "Bank account validation passed for IBAN {IBAN} (Account: {AccountNo})",
            iban,
            bankAccount.AccountNo);

        return true;
    }

    /// <summary>
    /// Validates that the account has sufficient available balance for the requested amount.
    /// Takes into account the allowed balance to use, not the full balance.
    /// </summary>
    /// <param name="senderAccount">The account to check for sufficient funds</param>
    /// <param name="totalAmount">The total amount required (including fees)</param>
    /// <param name="result">The API result object to populate with errors</param>
    /// <returns>True if sufficient funds are available; otherwise false</returns>
    public bool HasSufficientFunds(
        BankAccount? senderAccount,
        decimal totalAmount,
        ApiResult<Unit> result)
    {
        ArgumentNullException.ThrowIfNull(senderAccount);
        ArgumentNullException.ThrowIfNull(result);

        var availableBalance = senderAccount.AllowedBalanceToUse;

        if (availableBalance < totalAmount)
        {
            _logger.LogWarning(
               "Insufficient funds for IBAN {IBAN}: Required {Required}, Available {Available}",
               senderAccount.IBAN,
               totalAmount,
               availableBalance);

            result.AddError(ErrorCode.InSufficintFunds, 
                CashTransactionErrorMessages.InsufficientFunds);
            return false;
        }

        _logger.LogDebug(
          "Sufficient funds validation passed for IBAN {IBAN}: Required {Required}, Available {Available}",
          senderAccount.IBAN,
          totalAmount,
          availableBalance);

        return true;
    }
}