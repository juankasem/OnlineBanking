using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineBanking.Application.Models;
using OnlineBanking.Application.Models.Auth.Requests;
using OnlineBanking.Application.Models.Auth.Responses;
using OnlineBanking.Core.Domain.Entities;

namespace OnlineBanking.API.Controllers;

[Authorize]
public class AuthController : BaseApiController
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AuthController(ILogger<AuthController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {

        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Username);

        if (user == null) return Unauthorized();

        return Ok();
    }
}