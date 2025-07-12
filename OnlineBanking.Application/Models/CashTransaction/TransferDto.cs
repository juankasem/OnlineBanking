
namespace OnlineBanking.Application.Models.CashTransaction;

internal class TransferDto(string senderFullName, string recipientFullName,
                            decimal senderBalance, decimal recipientBalance,
                            decimal fees)
{

    public string SenderFullName { get; set; } = senderFullName;

    public string RecipientFullName { get; set; } = recipientFullName;

    public decimal SenderBalance { get; set; } = senderBalance;

    public decimal RecipientBalance { get; set; } = recipientBalance;

    public decimal Fees { get; set; } = fees;
}
