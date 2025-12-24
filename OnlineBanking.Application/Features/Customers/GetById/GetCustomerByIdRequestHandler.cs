
using OnlineBanking.Application.Models.Customer.Responses;

namespace OnlineBanking.Application.Features.Customers.GetById;

public class GetCustomerByIdRequestHandler : IRequestHandler<GetCustomerByIdRequest, ApiResult<CustomerResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCustomerByIdRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<CustomerResponse>> Handle(GetCustomerByIdRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<CustomerResponse>();

        var customer = await _uow.Customers.GetByIdAsync(request.CustomerId);

        if (customer is null)
        {
            result.AddError(ErrorCode.NotFound,
                string.Format(CustomerErrorMessages.NotFound, "Id", request.CustomerId));

            return result;
        }

        var customerBankAccounts = await _uow.Customers.GetCustomerBankAccountsAsync(customer.CustomerNo);

        var customerResponse = _mapper.Map<CustomerResponse>(customer);

        customerResponse.BankAccounts = _mapper.Map<IReadOnlyList<BankAccountDto>>(customerBankAccounts);

        result.Payload = customerResponse;

        return result;
    }
}