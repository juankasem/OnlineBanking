using AutoMapper;
using OnlineBanking.Application.Models.CreditCard;

namespace OnlineBanking.Application.Features.CreditCards.GetByCustomer;

public class GetCustomerCreditCardsRequestHandler : IRequestHandler<GetCustomerCreditCardsRequest, ApiResult<IReadOnlyList<CreditCardDto>>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCustomerCreditCardsRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<IReadOnlyList<CreditCardDto>>> Handle(GetCustomerCreditCardsRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<IReadOnlyList<CreditCardDto>>();

        var allCreditCards = await _uow.CreditCards.GetCustomerCreditCardsAsync(request.CustomerNo);

        if (!allCreditCards.Any())
            return result;

        var mappedCreditCards = allCreditCards.Select(creditCard => _mapper.Map<CreditCardDto>(creditCard))
                                                .ToList().AsReadOnly();

        result.Payload = mappedCreditCards;

        return result;

    }
}
