using AutoMapper;
using MediatR;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Features.FastTransactions.Messages;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.FastTransactions.CommandHandlers;

public class DeleteFastTransactionCommandHandler : IRequestHandler<DeleteFastTransactionCommand, ApiResult<Unit>>
{

    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IAppUserAccessor _appUserAccessor;

    public DeleteFastTransactionCommandHandler(IUnitOfWork uow, IMapper mapper, IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _mapper = mapper;
        _appUserAccessor = appUserAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(DeleteFastTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        try
        {
            var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.IBAN);

            if (bankAccount is null)
            {
                result.AddError(ErrorCode.NotFound,
                string.Format(FastTransactionErrorMessages.NotFound, "IBAN", request.IBAN));

                return result;
            }

            if (!bankAccount.BankAccountOwners.Any(b => b.Customer.AppUserId == loggedInAppUser.Id))
            {
                result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
                string.Format(FastTransactionErrorMessages.UnAuthorizedOperation, request.IBAN));

                return result;
            }

            //Delete fast transaction from account
            bankAccount.DelteFastTransaction(request.Id);

            //Update sender's account
            _uow.BankAccounts.Update(bankAccount);
            await _uow.SaveAsync();

            return result;
        }
        catch (FastTransactionNotValidException e)
        {
            e.ValidationErrors.ForEach(er => result.AddError(ErrorCode.ValidationError, er));
        }
        catch (Exception e)
        {
            result.AddUnknownError(e.Message);
        }

        return result;
    }
}