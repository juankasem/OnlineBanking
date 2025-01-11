using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts.Commands;
using OnlineBanking.Application.Features.Customers;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.BankAccount.CommandHandlers;

public class CreateBankAccountCommandHandler : IRequestHandler<CreateBankAccountCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    public CreateBankAccountCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResult<Unit>> Handle(CreateBankAccountCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        try
        {
            var bankAccount = CreateBankAccount(request);

            if (request.CustomerNos.Any())
            {
                var accountOwners = new List<Customer>();

                foreach (var customerNo in request.CustomerNos)
                {
                    var customer = await _uow.Customers.GetByCustomerNoAsync(customerNo);
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
                    var bankAccountOwner = CustomerBankAccount.Create(bankAccount.Id, accountOwner.Id);

                    bankAccount.AddOwnerToBankAccount(bankAccountOwner);
                }
            }

            _uow.BankAccounts.Add(bankAccount);

            await _uow.SaveAsync();

            return result;
        }
        catch (BankAccountNotValidException e)
        {
            e.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }

    #region Private helper methods
    private static Core.Domain.Aggregates.BankAccountAggregate.BankAccount CreateBankAccount(CreateBankAccountCommand request) =>
                Core.Domain.Aggregates.BankAccountAggregate.
                BankAccount.Create(request.AccountNo, request.IBAN, request.Type,
                                    request.BranchId, request.Balance,
                                    request.AllowedBalanceToUse, request.MinimumAllowedBalance,
                                    request.Debt, request.CurrencyId);
    #endregion
}
