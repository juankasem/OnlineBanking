using AutoMapper;
using OnlineBanking.Application.Features.CreditCards.Messages;
using OnlineBanking.Application.Models.CreditCard.Responses;

namespace OnlineBanking.Application.Features.CreditCards.GetById;

public class GetCreditCardByIdRequestHandler : IRequestHandler<GetCreditCardByIdRequest, ApiResult<CreditCardDetailsResponse>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetCreditCardByIdRequestHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<CreditCardDetailsResponse>> Handle(GetCreditCardByIdRequest request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<CreditCardDetailsResponse>();

        var creditCard = await _uow.CreditCards.GetByIdAsync(request.Id);

        if (creditCard is null)
        {
            result.AddError(ErrorCode.NotFound,
                string.Format(CreditCardsErrorMessages.NotFound, "No.", request.Id));

            return result;
        }

        result.Payload = _mapper.Map<CreditCardDetailsResponse>(creditCard);

        return result;
    }
}