
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate.Events;

namespace OnlineBanking.Application.Features.Customers.Create;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;


    public CreateCustomerCommandHandler(IUnitOfWork uow, 
                                        IMapper mapper, 
                                        ILogger<CreateCustomerCommandHandler> logger)
    {
        _uow = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResult<Unit>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting creating a new customer");

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

        // Add domain event
        customer.AddDomainEvent(new CustomerCreatedEvent(customer.Id,
            customer.CustomerNo,
            customer.FirstName,
            customer.LastName,
            customer.BirthDate,
            customer.Gender,
            customer.Nationality));

        // Persist changes
        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation("Customer of no. {customerNo} of name: {firstName} " +
                                    "{lastName}, identification no. {identificationNo}, " +
                                    "identification type: {identificationType}, " +
                                    "birth date: {birthDate}, gender: {gender}, " +
                                    "nationality: {nationality} is created successfully",
                                    customer.CustomerNo,
                                    customer.FirstName,
                                    customer.LastName,
                                    customer.IdentificationNo,
                                    customer.IdentificationType,
                                    customer.BirthDate,
                                    customer.Gender,
                                    customer.Nationality);
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CustomerErrorMessages.Unknown);
            _logger.LogError($"Creating customer failed!");
        }

        return result;
    }

    #region Private methods
    private static Customer CreateCustomer(CreateCustomerCommand request)
    {
        return Customer.Create(request.IdentificationNo, 
                                request.IdentificationType,
                                request.CustomerNo, 
                                request.AppUserId,
                                request.FirstName, 
                                request.MiddleName, 
                                request.LastName,
                                request.Nationality, 
                                request.Gender,
                                request.BirthDate, 
                                request.TaxNumber);
    }

    #endregion
}