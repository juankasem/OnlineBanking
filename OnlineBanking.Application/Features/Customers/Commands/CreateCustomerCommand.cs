using MediatR;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Address;
using OnlineBanking.Core.Domain.Enums;

namespace OnlineBanking.Application.Features.Customers.Commands;

public class CreateCustomerCommand : IRequest<ApiResult<Unit>>
{
    public string AppUserId { get; set; }

    public string IdentificationNo { get; set; }

    public IdentificationType IdentificationType { get; set; }

    public string CustomerNo { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public string Nationality { get; set; }

    public Gender Gender { get; set; }

    public DateTime BirthDate { get; set; }

    public string TaxNumber { get; set; }

    public AddressDto Address { get; set; }
}