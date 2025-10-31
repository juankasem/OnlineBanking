
using MediatR;
using Microsoft.Extensions.Logging;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Enums;
using OnlineBanking.Application.Features.BankAccounts;
using OnlineBanking.Application.Features.CashTransactions.Commands;
using OnlineBanking.Application.Helpers;
using OnlineBanking.Application.Models;
using OnlineBanking.Core.Domain.Enums;
using OnlineBanking.Core.Domain.Services.BankAccount;

namespace OnlineBanking.Application.Features.CashTransactions.CommandHandlers;

public class MakeDepositCommandHandler(IUnitOfWork uow,
                                        IBankAccountService bankAccountService,
                                        IAppUserAccessor appUserAccessor,
                                        ILogger<MakeDepositCommandHandler> logger) : 
                                  IRequestHandler<MakeDepositCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly ILogger<MakeDepositCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(MakeDepositCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Start creating deposit to {request.To}");

        var result = new ApiResult<Unit>();

        var userName = _appUserAccessor.GetUsername();
        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.BaseCashTransaction.IBAN);

        if (bankAccount is null)
        {
            result.AddError(ErrorCode.BadRequest,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.To));
            return result;
        }

        var bankAccountOwner = bankAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

        if (bankAccountOwner is null)
        {
            result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
            string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, request.BaseCashTransaction.IBAN));
            return result;
        }

        var amountToDeposit = request.BaseCashTransaction.Amount.Value;

        //Update account balance & Add transaction
        var updatedBalance = bankAccount.Balance + amountToDeposit;
        var recipient = bankAccountOwner.FirstName + " " + bankAccountOwner.LastName;

        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, recipient, updatedBalance);

        bool createdTransaction = _bankAccountService.CreateCashTransaction(null, bankAccount, cashTransaction);

        if (!createdTransaction)
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            return result;
        }

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            cashTransaction.UpdateStatus(CashTransactionStatus.Completed);
            _uow.BankAccounts.Update(bankAccount);

            await _uow.SaveAsync();

            _logger.LogInformation($"Deposit transaction of Id {cashTransaction.Id} of amount {amountToDeposit} is created");
        }
        else
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError($"Deposit transactyion failed");
        }

        return result;
    }
}
