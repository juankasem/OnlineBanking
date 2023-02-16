
using OnlineBanking.Application.Models.FastTransaction.Base;

namespace OnlineBanking.Application.Models.FastTransaction.Responses;

public class FastTransactionResponse : BaseFastTransactionDto
{
    public string RecipientBankName { get; set; }
}
