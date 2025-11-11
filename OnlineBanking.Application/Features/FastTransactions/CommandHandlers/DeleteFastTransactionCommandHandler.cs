using OnlineBanking.Application.Features.FastTransactions.Commands;
using OnlineBanking.Application.Features.FastTransactions.Messages;
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.FastTransactions.CommandHandlers;

public class DeleteFastTransactionCommandHandler(IUnitOfWork uow,
                                                 IBankAccountService bankAccountService,
                                                 ILogger<DeleteFastTransactionCommandHandler> logger,
                                                 IAppUserAccessor appUserAccessor) : IRequestHandler<DeleteFastTransactionCommand, ApiResult<Unit>>
{

    private readonly IUnitOfWork _uow = uow;
    private readonly IBankAccountService _bankAccountService = bankAccountService;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;
    private readonly ILogger<DeleteFastTransactionCommandHandler> _logger = logger;

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
            var fastTransactionDeleted = _bankAccountService.DeleteFastTransation(request.Id, bankAccount);

            if (!fastTransactionDeleted)
            {
                result.AddError(ErrorCode.UnknownError, FastTransactionErrorMessages.UnknownError);

                return result;
            }

            //Update sender's account
            if (await _uow.CompleteDbTransactionAsync() >= 1)
            {
                _logger.LogInformation($"Fast transaction of Id {request.Id} is successfully deleted");
            }
            else
            {
                result.AddError(ErrorCode.UnknownError, FastTransactionErrorMessages.UnknownError);
                _logger.LogError($"Ceate fast transaction failed...Please try again.");
            }

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