using OnlineBanking.Application.Models.Customer.Responses;

namespace OnlineBanking.Application.Features.Customers.Queries;

public class GetCustomerByIdRequest : IRequest<ApiResult<CustomerResponse>>
{
    public Guid CustomerId { get; set; }
}