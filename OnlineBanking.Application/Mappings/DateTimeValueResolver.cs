using AutoMapper;
using OnlineBanking.Application.Features.Customers.Commands;
using OnlineBanking.Application.Models.Customer.Requests;

using System.Globalization;

namespace OnlineBanking.Application.Mappings;
public class DateTimeValueResolver : IValueResolver<CreateCustomerRequest, CreateCustomerCommand, DateTime>
{

    public DateTime Resolve(CreateCustomerRequest source, CreateCustomerCommand destination, DateTime destMember, ResolutionContext context)
    {
        return DateTime.TryParseExact(source.BirthDate,
                                     "dd/MM/yyyy",
                                     CultureInfo.InvariantCulture,
                                     DateTimeStyles.None,
                                     out DateTime parsedDate) ? parsedDate : DateTime.MinValue;
    }
}