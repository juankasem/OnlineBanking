using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.Customers.Queries;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Customer.Responses;

namespace OnlineBanking.Application.Features.Customers.QueryHandlers;

public class GetAllCustomersRequestHandler : IRequestHandler<GetAllCustomersRequest,ApiResult<CustomerListResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllCustomersRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<CustomerListResponse>> Handle(GetAllCustomersRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<CustomerListResponse>();

        var customers = await _uow.Customers.GetAllAsync();

        result.Payload = _mapper.Map<CustomerListResponse>(customers);

        return result;   
    }
}
