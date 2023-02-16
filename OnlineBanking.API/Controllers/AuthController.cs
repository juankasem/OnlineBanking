using System;
using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Constants;
using OnlineBanking.API.Extensions;
using OnlineBanking.Application.Contracts.Infrastructure;
using OnlineBanking.Application.Models.Auth.Requests;
using OnlineBanking.Application.Models.Auth.Responses;
using OnlineBanking.Core.Domain.Entities;

namespace OnlineBanking.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;

    public AuthController(ILogger<AuthController> logger, 
                         UserManager<AppUser> userManager,
                         SignInManager<AppUser> signInManager,
                         ITokenService tokenService)
    {

        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService =  tokenService;
    }

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<UserResponse>> GetCurrentUser(CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailFromClaimsPrincipalAsync(User);
   
        var userResponse = _mapper.Map<UserResponse>(user);
        userResponse.Token = _tokenService.CreateToken(user);

        return Ok(userResponse);
    }

    [HttpGet(ApiRoutes.AppUsers.EmailExists)]
    public async Task<ActionResult<bool>> CheckEmailExists([FromQuery] string email, CancellationToken cancellationToken = default)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    [Authorize]
    [HttpGet(ApiRoutes.AppUsers.Phone)]
    public async Task<ActionResult<string>> GetUserPhone(CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailFromClaimsPrincipalAsync(User);

        return user.PhoneNumber;
    }

    [HttpPost(ApiRoutes.AppUsers.Login)]
    [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password)) 
        return Unauthorized();
        
        var userResponse = _mapper.Map<UserResponse>(user);
        userResponse.Token = _tokenService.CreateToken(user);

        return Ok(userResponse);
    }

    [HttpPost(ApiRoutes.AppUsers.Signup)]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult> Signup(SignupRequest request, CancellationToken cancellationToken = default)
    {
        var appUser = new AppUser()
        {
            UserName = request.Username,
            DisplayName = request.DisplayName,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(appUser, request.Password);

        if (!result.Succeeded)
         return BadRequest();

        await _userManager.AddToRoleAsync(appUser, Roles.User);

        var userResponse = _mapper.Map<UserResponse>(appUser);
        userResponse.Token = _tokenService.CreateToken(appUser);

        return StatusCode(201, userResponse);
    }
}