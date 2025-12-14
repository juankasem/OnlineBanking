
namespace OnlineBanking.Application.Models.CashTransaction;

internal record TransferDto(string RecipientFullName, decimal SenderBankAccountBalance, decimal RecipientBankAccountBalance, decimal Fees);
