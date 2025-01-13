
using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Customer.Responses;
using OnlineBanking.Core.Helpers;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Features.Customers.Queries;

public class GetAllCustomersRequest : IRequest<ApiResult<PagedList<CustomerResponse>>>
{
    public CustomerParams CustomerParams { get; set; }
}