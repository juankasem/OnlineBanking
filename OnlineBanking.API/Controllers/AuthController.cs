using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBanking.API.Common;
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
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;

    public AuthController(ILogger<AuthController> logger,
                        UserManager<AppUser> userManager,
                        SignInManager<AppUser> signInManager,
                        RoleManager<IdentityRole> roleManager,
                        ITokenService tokenService)
    {

        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    [Authorize]
    [HttpGet(ApiRoutes.AppUsers.CurrentUser)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AuthResponse>> GetCurrentUser()
    {
        var user = await _userManager.FindByEmailFromClaimsPrincipalAsync(User);

        var authClaims = new List<Claim>()
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.DisplayName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var userResponse = _mapper.Map<AuthResponse>(user);
        userResponse.Token = _tokenService.CreateToken(authClaims);

        return Ok(userResponse);
    }

    [HttpGet(ApiRoutes.AppUsers.EmailExists)]
    public async Task<ActionResult<bool>> CheckEmailExists([FromQuery] string email)
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
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        _logger.LogInformation($"Login attempt for {request.Username}");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByNameAsync(request.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.GivenName, user.DisplayName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.CreateToken(authClaims);
            user.RefreshToken = _tokenService.GenerateRefreshToken();

            var userResponse = _mapper.Map<AuthResponse>(user);
            userResponse.Token = token;

            return Ok(userResponse);
        }

        return Unauthorized();
    }


    [HttpPost(ApiRoutes.AppUsers.Signup)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Signup([FromBody] SignupRequest request)
    {
        _logger.LogInformation($"Registration attempt for {request.Email}");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = AppUser.Create(request.Username, request.DisplayName, request.Email, request.PhoneNumber);

        var result = await _userManager.CreateAsync(appUser, request.Password);

        if (!result.Succeeded)
            return BadRequest();

        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            await _userManager.AddToRoleAsync(appUser, UserRoles.User);

        if (request.IsAdmin){
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            await _userManager.AddToRoleAsync(appUser, UserRoles.Admin);
        }

        var userResponse = _mapper.Map<AuthResponse>(appUser);

        return StatusCode(201);
    }


    [HttpPost(ApiRoutes.AppUsers.RefreshToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var apiError = new ErrorResponse();

        string accessToken = request?.AccessToken;
        string refreshToken = request?.RefreshToken;

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

        if (principal == null)
            return BadRequest("Invalid access token or refresh token");

        string username = principal.Identity?.Name;

        var user = await _userManager.FindByNameAsync(username);
        _logger.LogInformation($"Refresh Token attempt for user: {user}");

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            return BadRequest("Invalid access token or refresh token");
        

        var newAccessToken = _tokenService.CreateToken(principal.Claims.ToList());
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return Ok(new
        {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken
        });
    }


    [Authorize]
    [HttpPost(ApiRoutes.AppUsers.Revoke)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return BadRequest("Invalid user name");

        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }


    [HttpPost(ApiRoutes.AppUsers.AssignRole)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignRoleToUser(AssignRoleToUserRequest request)
    {
        _logger.LogInformation($"Attempt to assign role to user: {request.Email}");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = await _userManager.FindByEmailAsync(request.Email);

        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        await _userManager.AddToRoleAsync(appUser, UserRoles.User);

        if (request.IsAdmin)
        {
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            await _userManager.AddToRoleAsync(appUser, UserRoles.Admin);
        }

        return Ok($"Role(s) are assigned Suessfully to username: {appUser.UserName}");
    }
}