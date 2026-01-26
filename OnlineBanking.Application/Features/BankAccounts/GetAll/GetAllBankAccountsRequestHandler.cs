using OnlineBanking.Application.Extensions;

namespace OnlineBanking.Application.Features.BankAccounts.GetAll;

public class GetAllBankAccountsRequestHandler : 
    IRequestHandler<GetAllBankAccountsRequest, ApiResult<PagedList<BankAccountDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IBankAccountMapper _bankAccountMapper;
    public GetAllBankAccountsRequestHandler(IUnitOfWork uow, IBankAccountMapper bankAccountMapper)
    {
        _uow = uow;
        _bankAccountMapper = bankAccountMapper;
    }

    public async Task<ApiResult<PagedList<BankAccountDto>>> Handle(GetAllBankAccountsRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<BankAccountDto>>();
        var bankAccountParams = request.BankAccountParams;

        var (bankAccounts, totalCount) = await _uow.BankAccounts.GetAllBankAccountsAsync(bankAccountParams);

        var mappedBankAccounts = bankAccounts.Select(bankAccount => _bankAccountMapper.MapToDtoModel(bankAccount))
                                             .ToList()
                                             .AsReadOnly();

        result.Payload = mappedBankAccounts.ToPagedList(
            totalCount, 
            bankAccountParams.PageNumber, 
            bankAccountParams.PageSize,
            cancellationToken);

        return result;
    }
}