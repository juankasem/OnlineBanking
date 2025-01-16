
using System.Globalization;

namespace OnlineBanking.Application.Common.Helpers;

public static class DateTimeHelper
{
    public static DateTime ConvertToDate(string source)
    {
        return DateTime.TryParseExact(source,
                            "dd/MM/yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out DateTime parsedDate) ? parsedDate : DateTime.MinValue;
    }
   
}