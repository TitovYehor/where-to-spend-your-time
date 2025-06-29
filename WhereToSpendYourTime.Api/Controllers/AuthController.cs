using Microsoft.AspNetCore.Mvc;
using WhereToSpendYourTime.Api.Models.Auth;
using WhereToSpendYourTime.Api.Services.Auth;

namespace WhereToSpendYourTime.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        this._authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var (succeeded, errors) = await _authService.RegisterAsync(request);
        if (!succeeded)
        {
            return BadRequest(errors);
        }

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        bool success = await _authService.LoginAsync(request);
        if (!success)
        {
            return Unauthorized("Invalid credentials");
        }

        return Ok();
    }
}
