using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.FastTransactions.Queries;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.FastTransaction.Responses;

namespace OnlineBanking.Application.Features.FastTransactions.QueryHandlers;

public class GetFastTransactionsByIBANRequestHandler : IRequestHandler<GetFastTransactionsByIBANRequest, ApiResult<IReadOnlyList<FastTransactionResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetFastTransactionsByIBANRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }


    public async Task<ApiResult<IReadOnlyList<FastTransactionResponse>>> Handle(GetFastTransactionsByIBANRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<IReadOnlyList<FastTransactionResponse>>();

        if (!await _uow.BankAccounts.ExistsAsync(request.IBAN))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "No.", request.IBAN));

            return result;
        }

        var accountFastTransactions = await _uow.FastTransactions.GetByIBANAsync(request.IBAN);

        if (!accountFastTransactions.Any())
        {
            return result;
        }

        result.Payload = accountFastTransactions.Select(aft => _mapper.Map<FastTransactionResponse>(aft))
                                                .ToList()
                                                .AsReadOnly();

        return result;
    }
}
