
namespace OnlineBanking.API.Filters;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ValidateBankAccountOwnerAttribute : TypeFilterAttribute
{
    public ValidateBankAccountOwnerAttribute(params string[] keys)
        : base(typeof(ValidateBankAccountOwnerFilter))
    {
        // Pass route/query/body keys to the filter (e.g., "IBAN", "accountNo")
        Arguments = [keys];
    }
}

public class ValidateBankAccountOwnerFilter(string[] keys, IUnitOfWork uow, IAppUserAccessor appUserAccessor) : IAsyncAuthorizationFilter
{
    private readonly string[] _keys = keys ?? [];
    private readonly IUnitOfWork _uow = uow;
    private readonly IAppUserAccessor _appUserAccessor = appUserAccessor;

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Get logged in app user
        var userName = _appUserAccessor.GetUsername();

        if (string.IsNullOrEmpty(userName))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var loggedInAppUser = await _uow.AppUsers.GetAppUser(userName);
        if (loggedInAppUser == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        string? accountNoOrIBAN = null;
        foreach (var key in _keys)
        {
            if (context.RouteData.Values.TryGetValue(key, out var val) && val is not null)
            {
                accountNoOrIBAN = val.ToString();
                break;
            }
        }

        if (string.IsNullOrEmpty(accountNoOrIBAN))
        {
            // No identifier found in route or query — let the action handle validation or return BadRequest
            context.Result = new BadRequestObjectResult(new ApiResult<Unit> { IsError = true });
            return;
        }

        // Check if the bank account with the given iban exists
        var bankAccount = await _uow.BankAccounts.GetByAccountNoOrIBANAsync(accountNoOrIBAN);
        if (bankAccount is null)
        {
            var notFoundResult = new ApiResult<Unit>();
            notFoundResult.AddError(ErrorCode.BadRequest, string.Format(BankAccountErrorMessages.NotFound, "Account No. or IBAN", accountNoOrIBAN));
            context.Result = new ObjectResult(notFoundResult) { StatusCode = StatusCodes.Status404NotFound };
            return;
        }

        // Check if the logged-in user is the owner of the bank account
        var isOwner = bankAccount.BankAccountOwners.Any(c => c.Customer.AppUserId == loggedInAppUser.Id);

        if (!isOwner)
        {
            var result = new ApiResult<Unit>();
            result.AddError(ErrorCode.CreateCashTransactionNotAuthorized,
                string.Format(CashTransactionErrorMessages.UnAuthorizedOperation, userName));

            context.Result = new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }
    }
}