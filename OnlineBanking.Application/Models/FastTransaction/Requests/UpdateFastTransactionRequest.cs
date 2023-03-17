using OnlineBanking.Application.Models.FastTransaction.Base;

namespace OnlineBanking.Application.Models.FastTransaction.Requests;

public class UpdateFastTransactionRequest : BaseFastTransactionDto
{
  public Guid Id { get; set; }

}
