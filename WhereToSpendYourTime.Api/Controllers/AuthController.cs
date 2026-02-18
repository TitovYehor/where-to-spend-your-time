using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Auth;
using WhereToSpendYourTime.Api.Services.Auth;

namespace WhereToSpendYourTime.Api.Controllers;

/// <summary>
/// Handles user authentication operations such as registration, login, and logout
/// </summary>
/// <remarks>
/// Base route: api/auth
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        this._authService = authService;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    /// <param name="request">Registration data containing email, password and display name</param>
    /// <returns>Status 201 if registration succeeds</returns>
    /// <response code="201">User successfully registered</response>
    /// <response code="400">Invalid registration data</response>
    /// <response code="409">User with given email already exists</response>
    /// <response code="500">User role assignment failed</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        await _authService.RegisterAsync(request);
        return Ok();
    }

    /// <summary>
    /// Authenticates a user using email and password
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>Status 200 if authentication succeeds</returns>
    /// <response code="200">User successfully authenticated</response>
    /// <response code="400">Invalid login request</response>
    /// <response code="401">Invalid email or password</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        await _authService.LoginAsync(request);
        return Ok();
    }

    /// <summary>
    /// Logs out the currently authenticated user
    /// </summary>
    /// <returns>Status 200 if logout succeeds</returns>
    /// <response code="200">User successfully logged out</response>
    /// <response code="401">User is not authenticated</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return Ok();
    }
}