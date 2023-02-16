using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Features.FastTransactions.Messages;
using OnlineBanking.Application.Features.FastTransactions.Validators;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.FastTransactions.CommandHandlers;

public class CreateFastTransactionCommandHandler : IRequestHandler<CreateFastTransactionCommand, ApiResult<Unit>>
{

    private readonly IUnitOfWork _uow;
    private readonly IAppUserAccessor _appUserAccessor;

    public CreateFastTransactionCommandHandler(IUnitOfWork uow, IMapper mapper, IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _appUserAccessor = appUserAccessor;
    }


    public async Task<ApiResult<Unit>> Handle(CreateFastTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();
        var validator = new CreateFastTransactionCommandValidator();

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            validationResult.Errors.ForEach(er => result.AddError(ErrorCode.ValidationError, er.ErrorMessage));

            return result;
        }

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        //Start database transaction
        using var dbContextTransaction = await _uow.CreateDbTransactionAsync();

        try
        {
            var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.RecipientIBAN);

            if (bankAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.RecipientIBAN));

                return result;
            }

            if (!bankAccount.BankAccountOwners.Any(b => b.Customer.AppUserId == loggedInAppUser.Id))
            {
                result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
                string.Format(FastTransactionErrorMessages.UnAuthorizedOperation, request.RecipientIBAN));

                return result;
            }

            var fastTransaction = CreateFastTransaction(request.RecipientIBAN, request.RecipientName, request.Amount, bankAccount.Id);

            //Add transaction to sender's account
            bankAccount.AddFastTransaction(fastTransaction);

            //Update sender's account
            await _uow.BankAccounts.UpdateAsync(bankAccount);

            await dbContextTransaction.CommitAsync();

            return result;
        }
        catch (FastTransactionNotValidException e)
        {
            await dbContextTransaction.RollbackAsync();
            e.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception e)
        {
            await dbContextTransaction.RollbackAsync();
            result.AddUnknownError(e.Message);
        }

        return result;
    }
    
    #region  Private methods
    private FastTransaction CreateFastTransaction(string recipientIBAN, string recipientName, decimal amount, Guid bankAccountId) =>
      FastTransaction.Create(recipientIBAN, recipientName, amount, bankAccountId);
   #endregion
}
