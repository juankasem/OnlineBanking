using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Features.Customers.Create;

/// <summary>
/// Handles customer creation requests.
/// Validates customer uniqueness, maps address data, creates domain entity, and persists changes.
/// </summary>
/// <remarks>
/// Initializes a new instance of the handler.
/// </remarks>
public class CreateCustomerCommandHandler(IUnitOfWork uow,
    IMapper mapper,
    ILogger<CreateCustomerCommandHandler> logger) : 
    IRequestHandler<CreateCustomerCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<CreateCustomerCommandHandler> _logger = logger;

    /// <summary>
    /// Handles the customer creation request.
    /// </summary>
    public async Task<ApiResult<Unit>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = new ApiResult<Unit>();
        var customerNo = request.CustomerNo;

        _logger.LogInformation("Processing customer creation request for customer number: {CustomerNo}", customerNo);

        if (await _uow.Customers.ExistsAsync(customerNo))
        {
            result.AddError(ErrorCode.CustomerAlreadyExists,
            string.Format(CustomerErrorMessages.AlreadyExists, customerNo));
            return result;
        }

        // Map address DTO to domain entity
        var address = MapAddressFromRequest(request);
         
        // Persist customer
        var customer = CreateCustomer(request);
        customer.SetAddress(address);

        await _uow.Customers.AddAsync(customer);

        // Persist changes
        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _logger.LogInformation(
            "Customer created successfully - CustomerNo: {CustomerNo}, Name: {FirstName} {LastName}, " +
            "IdentificationNo: {IdentificationNo}, IdentificationType: {IdentificationType}, " +
            "BirthDate: {BirthDate}, Gender: {Gender}, Nationality: {Nationality}",
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
            _logger.LogError(
                "Failed to persist customer creation for customer number: {CustomerNo}. " +
                "Database transaction returned 0 rows affected",
                customerNo); result.AddError(ErrorCode.UnknownError, CustomerErrorMessages.Unknown);
        }

        return result;
    }

    #region Private Helper methods
    private static Customer CreateCustomer(CreateCustomerCommand request)
    {
        ArgumentNullException.ThrowIfNull(request);

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

    /// <summary>
    /// Maps address DTO to domain Address entity.
    /// </summary>
    private Address MapAddressFromRequest(CreateCustomerCommand request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var address = _mapper.Map<Address>(request.Address);
        ArgumentNullException.ThrowIfNull(address, nameof(address));

        return address;
    }
    #endregion
}