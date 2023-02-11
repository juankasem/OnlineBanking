using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OnlineBanking.Application.Contracts.Infrastructure;

namespace OnlineBanking.Infrastructure.Persistence;

public class AppUserAccessor : IAppUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AppUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string GetUsername() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
    
}