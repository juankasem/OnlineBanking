
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.BankAccounts.Queries;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount;
using OnlineBanking.Application.Models.BankAccount.Responses;
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

        var allBankAccounts = await _uow.BankAccounts.GetAllAsync(request.BankAccountParams);

        if (!allBankAccounts.Any())
            return result;

        var mappedBankAccounts = allBankAccounts.Select(ba => _bankAccountMapper.MapToResponseModel(ba));

        result.Payload = (PagedList<BankAccountDto>)mappedBankAccounts;

        return result;
    }
}