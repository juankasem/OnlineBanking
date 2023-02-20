using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using OnlineBanking.Application.Contracts.Persistence;

namespace OnlineBanking.Infrastructure.Security;

public class IsAccountOwnerRequirement : IAuthorizationRequirement
{
}

public class IsAccountOwnerRequirementHandler : AuthorizationHandler<IsAccountOwnerRequirement>
{
    private readonly IUnitOfWork _uow;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IsAccountOwnerRequirementHandler(IUnitOfWork uow, IHttpContextAccessor httpContextAccessor)
    {
        _uow = uow;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAccountOwnerRequirement requirement)
    {
        var appUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (appUserId == null) return Task.CompletedTask;
        
        var accountId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
                                                        .SingleOrDefault(x => x.Key == "id").Value.ToString());
    
        var customerId = _uow.Customers.GetByAppUserIdAsync(appUserId).Result.Id;

        if (customerId == null) return Task.CompletedTask;

        var accountOwner = _uow.CustomerAccounts.GetCustomerAccountAsync(customerId, accountId).Result;

        if (accountOwner == null) return Task.CompletedTask;

        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}