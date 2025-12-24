
using OnlineBanking.Application.Models.Auth.Requests;
using OnlineBanking.Application.Models.Auth.Responses;

namespace OnlineBanking.API.Controllers;

/// <summary>
/// API controller for authentication and user management.
/// Handles login, registration, token refresh, role assignment, and user profile operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the AuthController.
/// </remarks>
public class AuthController(ILogger<AuthController> logger,
                    UserManager<AppUser> userManager,
                    SignInManager<AppUser> signInManager,
                    RoleManager<IdentityRole> roleManager,
                    ITokenService tokenService) : BaseApiController
{
    private readonly ILogger<AuthController> _logger = logger;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SignInManager<AppUser> _signInManager = signInManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly ITokenService _tokenService = tokenService;

    #region User Profile Operations

    /// <summary>
    /// Retrieves the current authenticated user's profile.
    /// </summary>
    [Authorize]
    [HttpGet(ApiRoutes.AppUsers.CurrentUser)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> GetCurrentUser()
    {
        var user = await _userManager.FindByEmailFromClaimsPrincipalAsync(User);

        if (user is null)
            return Unauthorized("User not found");

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

    /// <summary>
    /// Checks if an email address is already registered.
    /// </summary>
    [HttpGet(ApiRoutes.AppUsers.EmailExists)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<bool>> CheckEmailExists([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required");

        var emailExists = await _userManager.FindByEmailAsync(email) is not null;

        return Ok(emailExists);
    }

    /// <summary>
    /// Retrieves the phone number of the current authenticated user.
    /// </summary>
    [Authorize]
    [HttpGet(ApiRoutes.AppUsers.Phone)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> GetUserPhone()
    {
        var user = await _userManager.FindByEmailFromClaimsPrincipalAsync(User);

        if (user is null)
            return Unauthorized("User not found");

        return Ok(user.PhoneNumber ?? string.Empty);
    }

    #endregion

    #region Authentication Operations

    /// <summary>
    /// Authenticates a user with username and password.
    /// Returns JWT access token and refresh token on successful authentication.
    /// </summary>
    [HttpPost(ApiRoutes.AppUsers.Login)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}",  request.Username);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByNameAsync(request.Username);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            _logger.LogWarning("Invalid login attempt for user: {Username}", request.Username);
            return Unauthorized("Invalid username or password");
        }
        
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
        await _userManager.UpdateAsync(user);

        var userResponse = _mapper.Map<AuthResponse>(user);
        userResponse.Token = token;

        _logger.LogInformation("User {Username} logged in successfully", user.UserName);
        return Ok(userResponse);
    }

    /// <summary>
    /// Registers a new user account.
    /// Assigns User role by default; Admin role if requested.
    /// </summary>
    [HttpPost(ApiRoutes.AppUsers.Signup)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Signup([FromBody] SignupRequest request)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _userManager.FindByEmailAsync(request.Email) is not null)
        {
            _logger.LogWarning("Registration failed: Email already exists - {Email}", request.Email);
            return BadRequest($"User with email {request.Email} already exists");
        }

        var appUser = AppUser.Create(request.Username, request.DisplayName, request.Email, request.PhoneNumber);
        var createUserResult = await _userManager.CreateAsync(appUser, request.Password);

        if (!createUserResult.Succeeded)
        {
            return BadRequest(HandleErrorResult(createUserResult, "Failed to create user"));
        }

        // Assign User role
        var userRoleAssigned = await AssignRoleToUser(appUser, UserRoles.User);
        if (!userRoleAssigned)
            return BadRequest($"Failed to assign {UserRoles.User} role");

        if (request.IsAdmin)
        {
            var adminRoleAssigned = await AssignRoleToUser(appUser, UserRoles.Admin);
            if (!adminRoleAssigned)
                return BadRequest($"Failed to assign {UserRoles.Admin} role");
        }

        _logger.LogInformation("Role(s) assigned successfully to user: {Username}", appUser.UserName);

        var userResponse = _mapper.Map<AuthResponse>(appUser);

        return Ok(userResponse);
    }

    #endregion

    #region Token Operations

    /// <summary>
    /// Refreshes an expired access token using a valid refresh token.
    /// </summary>
    [HttpPost(ApiRoutes.AppUsers.RefreshToken)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        var apiError = new ErrorResponse();

        if (request is null)
            return BadRequest("Refresh token request is required");

        string accessToken = request?.AccessToken;
        string refreshToken = request?.RefreshToken;

        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);

        if (principal is null)
        {
             _logger.LogWarning("Invalid access token provided for refresh");
            return BadRequest("Invalid access token or refresh token");
        }

        string username = principal.Identity?.Name;
        var user = await _userManager.FindByNameAsync(username);

        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            _logger.LogWarning("Invalid or expired refresh token for user: {Username}", username);
            return BadRequest("Invalid or expired refresh token");
        }

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

    /// <summary>
    /// Revokes the refresh token for a user, effectively logging them out.
    /// </summary>
    [Authorize]
    [HttpPost(ApiRoutes.AppUsers.Revoke)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Revoke(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest("Username is required");

        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            _logger.LogWarning("Revoke token attempt for non-existent user: {Username}", username);
            return BadRequest("Invalid username");
        }

        user.RefreshToken = null; 
        user.RefreshTokenExpiryTime = DateTime.MinValue;

        await _userManager.UpdateAsync(user);

        _logger.LogInformation("Refresh token revoked for user: {Username}", username);
        return NoContent();
    }

    #endregion

    #region Role Management

    /// <summary>
    /// Assigns role to a user.
    /// </summary>
    [HttpPost(ApiRoutes.AppUsers.AssignRole)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRoleToUser(AssignRoleToUserRequest request)
    {
        _logger.LogInformation("Assigning roles to user: {Email}", request.Email);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var appUser = await _userManager.FindByEmailAsync(request.Email);
        if (appUser is null)
        {
            _logger.LogWarning("Role assignment failed: User not found - {Email}", request.Email);
            return BadRequest($"User with email {request.Email} not found");
        }

        var roleName = request.RoleName;
        if (string.IsNullOrWhiteSpace(roleName))
        {
            _logger.LogWarning("Role assignment failed: Role name is required");
            return BadRequest("Role name is required");
        }

        // Assign User role
        var userRoleAssigned = await AssignRoleToUser(appUser, roleName);
        if (!userRoleAssigned)
            return BadRequest($"Failed to assign {roleName} role");

        _logger.LogInformation("Role {roleName} assigned successfully to user: {Username}", roleName, appUser.UserName);
        return Ok($"Role {roleName} is assigned successfully to user: {appUser.UserName}");
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Assigns a role to a user, creating the role if it doesn't exist.
    /// </summary>
    private async Task<bool> AssignRoleToUser(AppUser user, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!createRoleResult.Succeeded)
            {
                _logger.LogError("Failed to create role: {RoleName}", roleName);
                return false;
            }
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addToRoleResult.Succeeded)
        {
            _logger.LogError("Failed to add user {Username} to role {RoleName}", user.UserName, roleName);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Converts IdentityResult errors to an ErrorResponse.
    /// </summary>
    private ErrorResponse HandleErrorResult(IdentityResult result, string logMessage = "")
    {
        var errorResponse = new ErrorResponse();

        var errors = result.Errors.Select(e => e.Description).ToList();
        errors.ForEach(er => errorResponse.Errors.Add(er));

        _logger.LogError(
          $"{logMessage}. Errors: {string.Join(", ", errors)}"
      );

        return errorResponse;
    }

    #endregion
}

