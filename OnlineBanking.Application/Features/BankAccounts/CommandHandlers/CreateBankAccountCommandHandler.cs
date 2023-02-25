using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts.Commands;
using OnlineBanking.Application.Features.BankAccounts.Validators;
using OnlineBanking.Application.Features.Customers;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Aggregates.CustomerAggregate;
using OnlineBanking.Core.Domain.Exceptions;
using OnlineBanking.Core.Domain.Validators;

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

            if (request.AccountOwners.Any())
            {
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
    private OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.BankAccount CreateBankAccount(CreateBankAccountCommand request) =>
    OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate.
                BankAccount.Create(request.AccountNo, request.IBAN, request.Type,
                                    request.BranchId, request.AccountBalance.Balance,
                                    request.AccountBalance.AllowedBalanceToUse, request.AccountBalance.MinimumAllowedBalance,
                                    request.AccountBalance.Debt, request.CurrencyId);
    #endregion
}
