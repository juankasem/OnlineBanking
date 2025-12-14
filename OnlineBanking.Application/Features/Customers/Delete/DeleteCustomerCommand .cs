namespace OnlineBanking.Application.Features.Customers.Delete;

public class DeleteCustomerCommand : IRequest<ApiResult<Unit>>
{
    public Guid CustomerId { get; set; }
}