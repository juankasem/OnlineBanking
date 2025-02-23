
using MediatR;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Features.FastTransactions.Messages;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Aggregates.BankAccountAggregate;
using OnlineBanking.Core.Domain.Services.BankAccount;


namespace OnlineBanking.Application.Features.FastTransactions.CommandHandlers;

public class CreateFastTransactionCommandHandler : IRequestHandler<CreateFastTransactionCommand, ApiResult<Unit>>
{

    private readonly IUnitOfWork _uow;
    private readonly IBankAccountService _bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor;

    public CreateFastTransactionCommandHandler(IUnitOfWork uow, 
                                               IBankAccountService bankAccountService, 
                                               IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _bankAccountService = bankAccountService;
        _appUserAccessor = appUserAccessor;
    }

    public async Task<ApiResult<Unit>> Handle(CreateFastTransactionCommand request, CancellationToken cancellationToken)
    {
        var result = new ApiResult<Unit>();

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.IBAN);
            
        if (bankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.IBAN));

            return result;
        }

        var recipientBankAccount = await _uow.BankAccounts.GetByIBANAsync(request.RecipientIBAN);

        if (recipientBankAccount is null)
        {
            result.AddError(ErrorCode.NotFound,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.RecipientIBAN));

            return result;
        }

        if (!bankAccount.BankAccountOwners.Any(b => b.Customer.AppUserId == loggedInAppUser.Id))
        {
            result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
            string.Format(FastTransactionErrorMessages.UnAuthorizedOperation, loggedInAppUser.UserName));

            return result;
        }

        var fastTransaction = FastTransaction.Create(bankAccount.Id, request.RecipientIBAN, request.RecipientName, request.Amount);
        await _uow.FastTransactions.AddAsync(fastTransaction);

        //Add fast transaction to sender's account
        var createdFastTransaction = _bankAccountService.CreateFastTransaction(bankAccount, fastTransaction);

        if (!createdFastTransaction)
        {
            result.AddError(ErrorCode.UnknownError, FastTransactionErrorMessages.UnknownError);

            return result;
        }

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            _uow.BankAccounts.Update(bankAccount);

            await _uow.SaveAsync();
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, FastTransactionErrorMessages.UnknownError);
        }

        return result;
    }
}
