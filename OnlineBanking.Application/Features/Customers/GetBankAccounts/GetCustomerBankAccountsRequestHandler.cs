
namespace OnlineBanking.Application.Features.Customers.GetBankAccounts;

public class GetCustomerBankAccountsRequestHandler : IRequestHandler<GetCustomerBankAccountsRequest, ApiResult<List<BankAccountDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCustomerBankAccountsRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<List<BankAccountDto>>> Handle(GetCustomerBankAccountsRequest request, 
                                                              CancellationToken cancellationToken)
    {
        var result = new ApiResult<List<BankAccountDto>>();
        var customerNo = request.CustomerNo;

        var customer = await _uow.Customers.GetByCustomerNoAsync(customerNo);

        if (customer is null)
        {
            result.AddError(ErrorCode.NotFound,
                string.Format(CustomerErrorMessages.NotFound, "No.", customerNo));

            return result;
        }

        var bankAccounts = await _uow.Customers.GetCustomerBankAccountsAsync(customerNo);

        result.Payload = _mapper.Map<List<BankAccountDto>>(bankAccounts);

        return result;
    }
}