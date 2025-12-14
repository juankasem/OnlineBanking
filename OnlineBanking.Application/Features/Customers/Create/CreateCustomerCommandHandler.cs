using AutoMapper;
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Features.Customers.Create;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<Unit>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var address = _mapper.Map<Address>(request.Address);

        if (await _uow.Customers.ExistsAsync(request.CustomerNo))
        {
            result.AddError(ErrorCode.CustomerAlreadyExists,
            string.Format(CustomerErrorMessages.AlreadyExists, request.CustomerNo));

            return result;
        }

        var customer = CreateCustomer(request);
        customer.SetAddress(address);

        await _uow.Customers.AddAsync(customer);
        await _uow.SaveAsync();

        return result;
    }

    #region Private methods
    private static Customer CreateCustomer(CreateCustomerCommand request)
    {
        return Customer.Create(request.IdentificationNo, request.IdentificationType,
                                request.CustomerNo, request.AppUserId,
                                request.FirstName, request.MiddleName, request.LastName,
                                request.Nationality, request.Gender,
                                request.BirthDate, request.TaxNumber);
    }
    #endregion
}