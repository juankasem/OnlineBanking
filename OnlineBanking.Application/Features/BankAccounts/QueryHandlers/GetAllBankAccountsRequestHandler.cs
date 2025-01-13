using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.BankAccounts.Queries;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Core.Helpers;

namespace OnlineBanking.Application.Features.BankAccount.QueryHandlers;

public class GetAllBankAccountsRequestHandler : IRequestHandler<GetAllBankAccountsRequest, ApiResult<PagedList<BankAccountDto>>>
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

        result.Payload = PagedList<BankAccountDto>.Create(mappedBankAccounts, 
                                                          totalCount,
                                                          bankAccountParams.PageNumber, 
                                                          bankAccountParams.PageSize);

        return result;
    }
}