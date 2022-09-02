using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.Customers.Commands;
using OnlineBanking.Application.Models;

namespace OnlineBanking.Application.Features.Customers.CommandHandlers;

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

            await _uow.Customers.DeleteAsync(customer);
        }

        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}