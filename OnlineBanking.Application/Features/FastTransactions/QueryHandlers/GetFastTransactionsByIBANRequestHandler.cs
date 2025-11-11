using AutoMapper;
using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Features.FastTransactions.Queries;
using OnlineBanking.Application.Models.FastTransaction.Responses;

namespace OnlineBanking.Application.Features.FastTransactions.QueryHandlers;

public class GetFastTransactionsByIBANRequestHandler : IRequestHandler<GetFastTransactionsByIBANRequest, ApiResult<PagedList<FastTransactionResponse>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetFastTransactionsByIBANRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }


    public async Task<ApiResult<PagedList<FastTransactionResponse>>> Handle(GetFastTransactionsByIBANRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<FastTransactionResponse>>();

        if (!await _uow.BankAccounts.ExistsAsync(request.IBAN))
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.IBAN));

            return result;
        }

        var fastTransactionParams = request.FastTransactionParams;

        var (fastTransactions, totalCount) = await _uow.FastTransactions.GetByIBANAsync(request.IBAN, fastTransactionParams);

        if (!fastTransactions.Any())
        {
            return result;
        }

        var mappedFastTransactions = fastTransactions.Select(ft => _mapper.Map<FastTransactionResponse>(ft))
                                                    .ToList()
                                                    .AsReadOnly();

        result.Payload = mappedFastTransactions.ToPagedList(totalCount, fastTransactionParams.PageNumber, fastTransactionParams.PageSize);

        return result;
    }
}
