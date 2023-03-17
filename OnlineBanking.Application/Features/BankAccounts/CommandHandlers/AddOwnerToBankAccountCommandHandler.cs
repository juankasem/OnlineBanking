using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.BankAccounts.Commands;
using OnlineBanking.Application.Features.Customers;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;

namespace OnlineBanking.Application.Features.BankAccount.CommandHandlers;

public class AddOwnerToBankAccountCommandHandler : IRequestHandler<AddOwnerToBankAccountCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public AddOwnerToBankAccountCommandHandler(IUnitOfWork uow, IMapper mapper)
    {

        _uow = uow;
        _mapper = mapper;
    }
    public async Task<ApiResult<Unit>> Handle(AddOwnerToBankAccountCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var bankAccount = await _uow.BankAccounts.GetByIdAsync(request.BankAccountId);
        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "Id", request.BankAccountId));

            return result;
        }

        var accountOwners = new List<Customer>();

        foreach (var owner in request.AccountOwners)
        {
            var customer = await _uow.Customers.GetByIdAsync(owner.CustomerId);
            if (customer is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(CustomerErrorMessages.NotFound, "Id", customer.Id));

                return result;
            }
            accountOwners.Add(customer);
        }

        foreach (var accountOwner in accountOwners)
        {
            var bankAccountOwner = new CustomerBankAccount()
                {
                    Customer = accountOwner,
                    BankAccount = bankAccount,
                    BankAccountType = bankAccount.Type
                };

            bankAccount.AddOwnerToBankAccount(bankAccountOwner);
        }


        _uow.BankAccounts.Update(bankAccount);
        await _uow.SaveAsync();

        return result;
    }
}
