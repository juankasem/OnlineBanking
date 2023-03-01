using MediatR;

namespace OnlineBanking.Application.Features.CreditCards.Commands;

public class ActivateCreditCardCommand : IRequest<Unit>
{
    public Guid CreditCardId { get; set; }
}
