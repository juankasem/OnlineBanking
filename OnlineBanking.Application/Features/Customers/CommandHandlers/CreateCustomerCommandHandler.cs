using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.Customers.Commands;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.AddressAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.Customers.CommandHandlers;

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

        try
        {
            var address = _mapper.Map<Address>(request.Address);
            
            await _uow.Customers.AddAsync(CreateCustomer(request, address));

            return result;
        }
        catch (CustomerNotValidException e)
        {
            e.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    #region Private methods
    private Customer CreateCustomer(CreateCustomerCommand request, Address address)
    {
        return Customer.Create(request.IDNo, request.IDType,
                                request.CustomerNo, request.AppUserId,
                                request.FirstName, request.MiddleName, request.LastName,
                                request.Nationality, request.Gender,
                                request.BirthDate, request.TaxNumber, address);
    }
    #endregion
}