using OnlineBanking.Application.Models.Address;

namespace OnlineBanking.Application.Features.Customers.Create;

/// <summary>
/// Represents a request to create a new customer.
/// Contains all customer information including personal details and address.
/// </summary>
public class CreateCustomerCommand : IRequest<ApiResult<Unit>>
{
    /// <summary>
    /// Gets the application user ID associated with this customer.
    /// </summary>
    public string AppUserId { get; set; }

    /// <summary>
    /// Gets the customer's identification number (e.g., passport, ID card).
    /// </summary>
    public string IdentificationNo { get; set; }

    /// <summary>
    /// Gets the type of identification document.
    /// </summary>
    public IdentificationType IdentificationType { get; set; }

    /// <summary>
    /// Gets the unique customer number assigned by the bank.
    /// </summary>
    public string CustomerNo { get; set; }

    /// <summary>
    /// Gets the customer's first name.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets the customer's middle name(s).
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Gets the customer's last name.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets the customer's nationality.
    /// </summary>
    public string Nationality { get; set; }

    /// <summary>
    /// Gets the customer's gender.
    /// </summary>
    public Gender Gender { get; set; }

    /// <summary>
    /// Gets the customer's date of birth.
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// Gets the customer's tax identification number.
    /// </summary>
    public string TaxNumber { get; set; }

    /// <summary>
    /// Gets the customer's address information.
    /// </summary>
    public AddressDto Address { get; set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCustomerCommand"/> class.
    /// </summary>
    /// <param name="appUserId">The application user ID</param>
    /// <param name="identificationNo">The identification number</param>
    /// <param name="identificationType">The identification type</param>
    /// <param name="customerNo">The customer number</param>
    /// <param name="firstName">The first name</param>
    /// <param name="lastName">The last name</param>
    /// <param name="nationality">The nationality</param>
    /// <param name="gender">The gender</param>
    /// <param name="birthDate">The date of birth</param>
    /// <param name="taxNumber">The tax identification number</param>
    /// <param name="address">The address information</param>
    /// <param name="middleName">The middle name (optional)</param>
    /// <exception cref="ArgumentNullException">Thrown when required parameters are null</exception>
    /// <exception cref="ArgumentException">Thrown when required string parameters are empty or whitespace</exception>
    public CreateCustomerCommand(
        string appUserId,
        string identificationNo,
        IdentificationType identificationType,
        string customerNo,
        string firstName,
        string lastName,
        string nationality,
        Gender gender,
        DateTime birthDate,
        string taxNumber,
        AddressDto address,
        string? middleName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(appUserId);
        ArgumentException.ThrowIfNullOrWhiteSpace(identificationNo);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerNo);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(nationality);
        ArgumentException.ThrowIfNullOrWhiteSpace(taxNumber);
        ArgumentNullException.ThrowIfNull(address);

        if (birthDate == default)
            throw new ArgumentException("Birth date must be a valid date.", nameof(birthDate));

        AppUserId = appUserId;
        IdentificationNo = identificationNo;
        IdentificationType = identificationType;
        CustomerNo = customerNo;
        FirstName = firstName;
        MiddleName = middleName;
        LastName = lastName;
        Nationality = nationality;
        Gender = gender;
        BirthDate = birthDate;
        TaxNumber = taxNumber;
        Address = address;
    }
}