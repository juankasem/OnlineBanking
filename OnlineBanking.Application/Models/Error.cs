
using OnlineBanking.Application.Enums;

namespace OnlineBanking.Application.Models
{
    public class Error
    {
        public ErrorCode Code { get; set; }
        public string Message { get; set; }
    }
}