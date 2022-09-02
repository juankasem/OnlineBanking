using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts.Queries;
using OnlineBanking.Application.Features.Customers;
using OnlineBanking.Application.Mappings.BankAccounts;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.BankAccount.Responses;

namespace OnlineBanking.Application.Features.BankAccount.QueryHandlers;

public class GetBankAccountsByCustomerNoRequestHandler : IRequestHandler<GetBankAccountsByCustomerNoRequest, ApiResult<BankAccountListResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IBankAccountMapper _bankAccountMapper;

    public GetBankAccountsByCustomerNoRequestHandler(IUnitOfWork uow,
                                                    IMapper mapper,
                                                    IBankAccountMapper bankAccountMapper)
    {
        _uow = uow;
        _mapper = mapper;
        _bankAccountMapper = bankAccountMapper;
    }

    public async Task<ApiResult<BankAccountListResponse>> Handle(GetBankAccountsByCustomerNoRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<BankAccountListResponse>();

        if (!await _uow.Customers.ExistsAsync(request.CustomerNo))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(CustomerErrorMessages.NotFound, "Customer No.", request.CustomerNo));

            return result;
        }

        var customerAccounts = await _uow.BankAccounts.GetAccountsByCustomerNoAsync(request.CustomerNo);

        var mappedAccounts = customerAccounts.Select(ca => _bankAccountMapper.MapToResponseModel(ca.BankAccount))
                                            .ToImmutableList();

        result.Payload = new(mappedAccounts, mappedAccounts.Count());

        return result;
    }
}