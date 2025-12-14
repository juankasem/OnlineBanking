using AutoMapper;

namespace OnlineBanking.Application.Features.Customers.Delete;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public DeleteCustomerCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
    public async Task<ApiResult<Unit>> Handle(DeleteCustomerCommand request,
                                            CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        try
        {
            var customer = await _uow.Customers.GetByIdAsync(request.CustomerId);

            if (customer is null)
            {
                result.AddError(ErrorCode.NotFound,
                    string.Format(CustomerErrorMessages.NotFound, "Id", request.CustomerId));

                return result;
            }

            _uow.Customers.Delete(customer);
            await _uow.SaveAsync();
        }

        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}