using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineBanking.API.Common
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string? StatusPhrase { get; set; }
        public List<string> Errors { get; } = new List<string>();
        public DateTime Timestamp { get; set; }
    }
}