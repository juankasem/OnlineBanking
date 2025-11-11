using AutoMapper;
using OnlineBanking.Application.Features.Customers.Queries;
using OnlineBanking.Application.Models.BankAccount;

namespace OnlineBanking.Application.Features.Customers.QueryHandlers;

public class GetCustomerBankAccountsRequestHandler : IRequestHandler<GetCustomerBankAccountsRequest, ApiResult<List<BankAccountDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCustomerBankAccountsRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<BankAccountDto>>> Handle(GetCustomerBankAccountsRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<List<BankAccountDto>>();

        var customer = await _uow.Customers.GetByIdAsync(request.CustomerId);

        if (customer is null)
        {
            result.AddError(ErrorCode.NotFound,
                string.Format(CustomerErrorMessages.NotFound, "No.", request.CustomerId));

            return result;
        }

        var bankAccounts = await _uow.Customers.GetCustomerBankAccountsAsync(request.CustomerId);

        result.Payload = _mapper.Map<List<BankAccountDto>>(bankAccounts);

        return result;
    }
}