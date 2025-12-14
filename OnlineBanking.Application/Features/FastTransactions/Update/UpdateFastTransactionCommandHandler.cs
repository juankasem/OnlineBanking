
using OnlineBanking.Core.Domain.Exceptions;

namespace OnlineBanking.Application.Features.FastTransactions.Update;

public class UpdateFastTransactionCommandHandler : IRequestHandler<UpdateFastTransactionCommand, ApiResult<Unit>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IAppUserAccessor _appUserAccessor;

    public UpdateFastTransactionCommandHandler(IUnitOfWork uow, IMapper mapper, IAppUserAccessor appUserAccessor)
    {
        _uow = uow;
        _mapper = mapper;
        _appUserAccessor = appUserAccessor;
    }


    public async Task<ApiResult<Unit>> Handle(UpdateFastTransactionCommand request, CancellationToken cancellationToken)
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
                string.Format(FastTransactionErrorMessages.UnAuthorizedOperation, request.RecipientIBAN));

                return result;
            }

            var fastTransaction = FastTransaction.Create(bankAccount.Id, request.RecipientIBAN,
                                                         request.RecipientName, request.Amount, request.Id);

            //Add transaction to sender's account
            bankAccount.UpdateFastTransaction(request.Id, fastTransaction);

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
