namespace OnlineBanking.Application.Features.Customers.Commands;

public class DeleteCustomerCommand : IRequest<ApiResult<Unit>>
{
    public Guid CustomerId { get; set; }
}