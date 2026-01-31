using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WhereToSpendYourTime.Api.Exceptions.Auth;
using WhereToSpendYourTime.Api.Models.Auth;
using WhereToSpendYourTime.Api.Services.Auth;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userManagerMock = MockUserManager<ApplicationUser>();
        _signInManagerMock = MockSignInManager(_userManagerMock.Object);

        _authService = new AuthService(_userManagerMock.Object, _signInManagerMock.Object);
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
                Description = "Password is too short."
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