using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.Customers.Queries;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Customer.Responses;
using OnlineBanking.Core.Helpers;

namespace OnlineBanking.Application.Features.Customers.QueryHandlers;

public class GetAllCustomersRequestHandler : IRequestHandler<GetAllCustomersRequest,ApiResult<PagedList<CustomerResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllCustomersRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<PagedList<CustomerResponse>>> Handle(GetAllCustomersRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<CustomerResponse>>();
        var requestParams = request.CustomerParams;

        var customers = await _uow.Customers.GetAllAsync(request.CustomerParams);

        var mappedCustomers = _mapper.Map<IReadOnlyList<CustomerResponse>>(customers);

        result.Payload = PagedList<CustomerResponse>.Create(mappedCustomers, requestParams.PageNumber, requestParams.PageSize);

        return result;   
    }
}
