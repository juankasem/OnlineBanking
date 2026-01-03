
using Microsoft.AspNetCore.Http;

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
            var errorResponse = ErrorResponse.Create(StatusCodes.Status400BadRequest, 
                                                     ErrorPhrase.BadRequest,
                                                    [string.Format(BankAccountErrorMessages.NoIdentifierFound, 
                                                    "Account No. or IBAN", 
                                                    accountNoOrIBAN)
                                                    ]);
          
            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
            return;
        }

        // Check if the bank account with the given iban exists
        var bankAccount = await _uow.BankAccounts.GetByAccountNoOrIBANAsync(accountNoOrIBAN);
        if (bankAccount is null)
        {
            var errorResponse = ErrorResponse.Create(StatusCodes.Status400BadRequest,
                                                     ErrorPhrase.BadRequest,
                                                    [string.Format(BankAccountErrorMessages.NotFound,
                                                    "Account No. or IBAN",
                                                    accountNoOrIBAN)
                                                    ]);

            context.Result = new ObjectResult(errorResponse) 
            { 
                StatusCode = StatusCodes.Status404NotFound 
            };
            return;
        }

        // Check if the logged-in user is the owner of the bank account
        var isOwner = bankAccount.BankAccountOwners.Any(c => c.Customer.AppUserId == loggedInAppUser.Id);
        if (!isOwner)
        {
            var errorResponse = ErrorResponse.Create(StatusCodes.Status403Forbidden,
                                                     ErrorPhrase.Forbidden,
                                                    [string.Format(BankAccountErrorMessages.UnauthorizedOperation,
                                                    userName)
                                                    ]);

            context.Result = new ObjectResult(errorResponse)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
            return;
        }
    }
}