using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Features.Customers.Queries;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Customer.Responses;

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
        var customerParams = request.CustomerParams;

        var (customers, totalCount) = await _uow.Customers.GetAllAsync(request.CustomerParams);

        var mappedCustomers = _mapper.Map<IReadOnlyList<CustomerResponse>>(customers);

        result.Payload = mappedCustomers.ToPagedList(totalCount, customerParams.PageNumber, customerParams.PageSize); 

        return result;   
    }
}