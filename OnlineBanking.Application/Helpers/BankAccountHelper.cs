
using OnlineBanking.Application.Features.CashTransactions;

namespace OnlineBanking.Application.Helpers;

internal static class BankAccountHelper
{
    /// <summary>
    /// Validates that the bank account exists and is valid
    /// </summary>
    public static bool ValidateBankAccount(
       BankAccount? bankAccount, 
       string iban,
       ApiResult<Unit> result)
    {
        var success = true;

        if (bankAccount == null)
        {
            result.AddError(ErrorCode.BadRequest, string.Format(BankAccountErrorMessages.NotFound, "IBAN.", iban));
            success = false;
        }

        if (!bankAccount.IsActive)
        {
            result.AddError(ErrorCode.BadRequest,
                string.Format(BankAccountErrorMessages.Inactive, iban));
            return false;
        }

        return success;
    }

    /// <summary>
    /// Validates that the sender has sufficient funds for the transfer including fees
    /// </summary>
    public static bool HasSufficientFunds(
        BankAccount? senderAccount,
        decimal totalAmount,
        ApiResult<Unit> result)
    {
        if (senderAccount.AllowedBalanceToUse < totalAmount)
        {
            result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);
            return false;
        }

        return true;
    }
}