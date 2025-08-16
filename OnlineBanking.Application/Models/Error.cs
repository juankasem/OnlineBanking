
using OnlineBanking.Application.Enums;

namespace OnlineBanking.Application.Models;

public sealed record Error(ErrorCode Code, string Message)
{
    public static readonly Error None = new(ErrorCode.None, string.Empty);
}
   
