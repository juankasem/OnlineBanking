namespace OnlineBanking.Application.Helpers.Params;
public class CashTransactionParams : PaginationParams
{
    public string OrderBy { get; set; }
    public int TimeScope { get; set; } = 30;
}
