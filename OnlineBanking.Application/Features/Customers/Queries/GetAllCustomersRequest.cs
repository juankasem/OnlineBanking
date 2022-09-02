using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Customer.Responses;

namespace OnlineBanking.Application.Features.Customers.Queries;

public class GetAllCustomersRequest : IRequest<ApiResult<CustomerListResponse>>
{
}