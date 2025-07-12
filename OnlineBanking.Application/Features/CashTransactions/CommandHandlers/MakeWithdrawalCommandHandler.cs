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

    public class MakeWithdrawalCommandHandler(IUnitOfWork uow,
                                                IBankAccountService bankAccountService,
                                                IAppUserAccessor appUserAccessor,
                                                ILogger<MakeWithdrawalCommandHandler> logger) : 
                                                IRequestHandler<MakeWithdrawalCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly ILogger<MakeWithdrawalCommandHandler> _logger = logger;

    public async Task<ApiResult<Unit>> Handle(MakeWithdrawalCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Start creating withdrawal from {request.From}");

        var result = new ApiResult<Unit>();

        var loggedInAppUser = await _uow.AppUsers.GetAppUser(_appUserAccessor.GetUsername());

        var bankAccount = await _uow.BankAccounts.GetByIBANAsync(request.BaseCashTransaction.IBAN);

        if (bankAccount is null)
        {
            result.AddError(ErrorCode.BadRequest,
            string.Format(BankAccountErrorMessages.NotFound, "IBAN", request.From));

            return result;
        }

        var bankAccountOwner = bankAccount.BankAccountOwners.FirstOrDefault(c => c.Customer.AppUserId == loggedInAppUser.Id)?.Customer;

        if (bankAccountOwner is null)
        {
            result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
            string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, loggedInAppUser.UserName));

            return result;
        }

        var amountToWithdraw = request.BaseCashTransaction.Amount.Value;

        if (bankAccount.AllowedBalanceToUse < amountToWithdraw)
        {
            result.AddError(ErrorCode.InSufficintFunds, CashTransactionErrorMessages.InsufficientFunds);

            return result;
        }
        
        //Update account balance & Add transaction
        var updatedBalance = bankAccount.Balance - amountToWithdraw; 
        var sender = $"{bankAccountOwner.FirstName} {bankAccountOwner.LastName}";

        var cashTransaction = CashTransactionHelper.CreateCashTransaction(request, sender, updatedBalance);        
        await _uow.CashTransactions.AddAsync(cashTransaction);

        bool createdTransaction = _bankAccountService.CreateCashTransaction(bankAccount, null, 
                                                                            cashTransaction.Id, 
                                                                            amountToWithdraw, 0, 
                                                                            CashTransactionType.Withdrawal); 
        if (!createdTransaction)
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);

            return result;
        }

        _uow.BankAccounts.Update(bankAccount);

        if (await _uow.CompleteDbTransactionAsync() >= 1)
        {
            cashTransaction.UpdateStatus(CashTransactionStatus.Completed);
            _uow.CashTransactions.Update(cashTransaction);

            await _uow.SaveAsync();

            _logger.LogInformation($"Withdrawal transaction of Id {cashTransaction.Id} of amount {amountToWithdraw} is created");
        }
        else 
        {
            result.AddError(ErrorCode.UnknownError, CashTransactionErrorMessages.UnknownError);
            _logger.LogError($"Withdrawal transaction failed...Please try again.");
        }

        return result;
    }
}