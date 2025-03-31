using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Extensions;
using OnlineBanking.Application.Features.CreditCards.Queries;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CreditCard;

namespace OnlineBanking.Application.Features.CreditCards.QueryHandlers;

public class GetAllCreditCardsRequestHandler : IRequestHandler<GetAllCreditCardsRequest, ApiResult<PagedList<CreditCardDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetAllCreditCardsRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<PagedList<CreditCardDto>>> Handle(GetAllCreditCardsRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<PagedList<CreditCardDto>>();
        var creditCardParams = request.CreditCardParams;

        var (creditCards, totalCount) = await _uow.CreditCards.GetAllAsync(creditCardParams);

        if (!creditCards.Any())
            return result;

        var mappedCreditCards = creditCards.Select(creditCard => _mapper.Map<CreditCardDto>(creditCard))
                                           .ToList().AsReadOnly();

        result.Payload = mappedCreditCards.ToPagedList(totalCount, creditCardParams.PageNumber, creditCardParams.PageSize);

        return result;
    }
}