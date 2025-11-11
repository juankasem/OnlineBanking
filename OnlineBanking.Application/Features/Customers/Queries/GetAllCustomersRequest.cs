using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Models.Customer.Responses;

namespace OnlineBanking.Application.Features.Customers.Queries;

public class GetAllCustomersRequest : IRequest<ApiResult<PagedList<CustomerResponse>>>
{
    public CustomerParams CustomerParams { get; set; }
}