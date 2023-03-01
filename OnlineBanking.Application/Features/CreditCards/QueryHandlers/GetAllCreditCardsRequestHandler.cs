using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Features.CreditCards.Queries;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.CreditCard;
using OnlineBanking.Core.Helpers;

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
        var reqParams = request.CreditCardParams;

        var allCreditCards = await _uow.CreditCards.GetAllAsync(reqParams);

        if (!allCreditCards.Any())
            return result;

        var mappedCreditCards = allCreditCards.Select(creditCard => _mapper.Map<CreditCardDto>(creditCard))
                                                .ToList().AsReadOnly();

        result.Payload = PagedList<CreditCardDto>.Create(mappedCreditCards, reqParams.PageNumber, reqParams.PageSize);

        return result;
    }
}