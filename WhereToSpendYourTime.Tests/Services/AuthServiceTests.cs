using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WhereToSpendYourTime.Api.Exceptions.Auth;
using WhereToSpendYourTime.Api.Models.Auth;
using WhereToSpendYourTime.Api.Services.Auth;
using WhereToSpendYourTime.Api.Services.Email;
using WhereToSpendYourTime.Api.Services.Email.Config;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

/// <summary>
/// Unit tests for <see cref="AuthService"/>.
/// 
/// Verifies authentication behaviors such as:
/// - user registration
/// - login success
/// - login failure scenarios
/// - password reset request handling
/// - password reset execution and error handling
/// </summary>
public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IOptions<FrontendSettings>> _frontendSettingsMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = MockUserManager<ApplicationUser>();
        _signInManagerMock = MockSignInManager(_userManagerMock.Object);

        _emailServiceMock = new Mock<IEmailService>();

        _frontendSettingsMock = new Mock<IOptions<FrontendSettings>>();
        _frontendSettingsMock
            .Setup(opt => opt.Value)
            .Returns(new FrontendSettings
            {
                BaseUrl = "http://test-url"
            });

        _authService = new AuthService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _emailServiceMock.Object,
            _frontendSettingsMock.Object
        );
    }

    [Fact]
    public async Task RegisterAsync_DoesNotThrow_WhenUserIsCreated()
    {
        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Test123!",
            DisplayName = "TestUser"
        };

        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock
            .Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
            .ReturnsAsync(IdentityResult.Success);

        var exception = await Record.ExceptionAsync(() =>
            _authService.RegisterAsync(request));

        Assert.Null(exception);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsRegisterFailedException_WhenCreationFails()
    {
        var errors = new[]
        {
            new IdentityError
            {
                Code = "PasswordTooShort",
                Description = "Password is too short"
            }
        };

        _userManagerMock
            .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(errors));

        var request = new RegisterRequest
        {
            Email = "fail@example.com",
            Password = "123",
            DisplayName = "BadUser"
        };

        var ex = await Assert.ThrowsAsync<RegisterFailedException>(() =>
            _authService.RegisterAsync(request));

        Assert.Contains(ex.Errors, e => e.Code == "PasswordTooShort");
    }

    [Fact]
    public async Task LoginAsync_DoesNotThrow_WhenCredentialsAreValid()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            Email = "user@example.com"
        };

        _userManagerMock
            .Setup(um => um.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(sm => sm.PasswordSignInAsync(user, It.IsAny<string>(), true, false))
            .ReturnsAsync(SignInResult.Success);

        var request = new LoginRequest
        {
            Email = user.Email,
            Password = "ValidPass123"
        };

        var exception = await Record.ExceptionAsync(() =>
            _authService.LoginAsync(request));

        Assert.Null(exception);
    }

    [Fact]
    public async Task LoginAsync_ThrowsInvalidCredentialsException_WhenUserNotFound()
    {
        _userManagerMock
            .Setup(um => um.FindByEmailAsync("unknown@example.com"))
            .ReturnsAsync((ApplicationUser?)null);

        var request = new LoginRequest
        {
            Email = "unknown@example.com",
            Password = "123"
        };

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_ThrowsInvalidCredentialsException_WhenPasswordIsIncorrect()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            Email = "user@example.com"
        };

        _userManagerMock
            .Setup(um => um.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(sm => sm.PasswordSignInAsync(user, It.IsAny<string>(), true, false))
            .ReturnsAsync(SignInResult.Failed);

        var request = new LoginRequest
        {
            Email = user.Email,
            Password = "WrongPass"
        };

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _authService.LoginAsync(request));
    }

    [Fact]
    public async Task RequestPasswordResetAsync_Throws_WhenEmailIsEmpty()
    {
        await Assert.ThrowsAsync<InvalidPasswordResetRequestException>(() =>
            _authService.RequestPasswordResetAsync(""));
    }

    [Fact]
    public async Task RequestPasswordResetAsync_DoesNothing_WhenUserNotFound()
    {
        _userManagerMock
            .Setup(u => u.FindByEmailAsync("unknown@example.com"))
            .ReturnsAsync((ApplicationUser?)null);

        await _authService.RequestPasswordResetAsync("unknown@example.com");

        _emailServiceMock.Verify(es =>
            es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task RequestPasswordResetAsync_SendsEmail_WhenUserExists()
    {
        var user = new ApplicationUser { Email = "test@example.com" };

        _userManagerMock
            .Setup(u => u.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(u => u.GeneratePasswordResetTokenAsync(user))
            .ReturnsAsync("RESET_TOKEN");

        var templatePath = Path.Combine("Templates", "PasswordResetEmail.html");
        Directory.CreateDirectory("Templates");
        await File.WriteAllTextAsync(templatePath, "Click here: {{resetLink}}");

        string? capturedHtml = null;

        _emailServiceMock
            .Setup(es => es.SendEmailAsync(
                user.Email,
                "Reset your password",
                It.IsAny<string>()))
            .Callback<string, string, string>((_, _, body) => capturedHtml = body)
            .Returns(Task.CompletedTask);

        await _authService.RequestPasswordResetAsync(user.Email);

        Assert.NotNull(capturedHtml);
        Assert.Contains("http://test-url/reset-password-confirm?email=test@example.com&token=", capturedHtml);
    }

    [Fact]
    public async Task ResetPasswordAsync_Throws_WhenParametersMissing()
    {
        await Assert.ThrowsAsync<InvalidPasswordResetTokenException>(() =>
            _authService.ResetPasswordAsync("", "token", "pass"));

        await Assert.ThrowsAsync<InvalidPasswordResetTokenException>(() =>
            _authService.ResetPasswordAsync("email@example.com", "", "pass"));

        await Assert.ThrowsAsync<InvalidPasswordResetTokenException>(() =>
            _authService.ResetPasswordAsync("email@example.com", "token", ""));
    }

    [Fact]
    public async Task ResetPasswordAsync_Throws_WhenUserNotFound()
    {
        _userManagerMock
            .Setup(u => u.FindByEmailAsync("missing@example.com"))
            .ReturnsAsync((ApplicationUser?)null);

        await Assert.ThrowsAsync<InvalidPasswordResetTokenException>(() =>
            _authService.ResetPasswordAsync("missing@example.com", "token", "NewPass1!"));
    }

    [Fact]
    public async Task ResetPasswordAsync_Throws_WhenResetFails()
    {
        var user = new ApplicationUser { Email = "test@example.com" };

        _userManagerMock
            .Setup(u => u.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(u => u.ResetPasswordAsync(user, "token123", "NewPass1!"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError
            {
                Code = "BadToken",
                Description = "Invalid token"
            }));

        await Assert.ThrowsAsync<PasswordResetFailedException>(() =>
            _authService.ResetPasswordAsync(user.Email, "token123", "NewPass1!"));
    }

    [Fact]
    public async Task ResetPasswordAsync_DoesNotThrow_WhenSuccessful()
    {
        var user = new ApplicationUser { Email = "test@example.com" };

        _userManagerMock
            .Setup(u => u.FindByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _userManagerMock
            .Setup(u => u.ResetPasswordAsync(user, "valid-token", "Password123!"))
            .ReturnsAsync(IdentityResult.Success);

        var ex = await Record.ExceptionAsync(() =>
            _authService.ResetPasswordAsync(user.Email, "valid-token", "Password123!"));

        Assert.Null(ex);
    }

    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(
            store.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<TUser>>().Object,
            Array.Empty<IUserValidator<TUser>>(),
            Array.Empty<IPasswordValidator<TUser>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<TUser>>>().Object
        );
    }

    private static Mock<SignInManager<TUser>> MockSignInManager<TUser>(UserManager<TUser> userManager) where TUser : class
    {
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();
        return new Mock<SignInManager<TUser>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<TUser>>>().Object,
            new Mock<IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<TUser>>().Object
        );
    }
}