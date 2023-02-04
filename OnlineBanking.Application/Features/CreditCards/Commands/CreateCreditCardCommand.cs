
using MediatR;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.CreditCards.Commands;

public class CreateCreditCardCommand : IRequest<ApiResult<Unit>>
{
    public string CustomerId { get; set; }

    public int MyProperty { get; set; }
}
