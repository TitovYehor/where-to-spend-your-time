using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
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
    public async Task RegisterAsync_ShouldReturnSuccess_WhenUserIsCreated()
    {
        var request = new RegisterRequest { Email = "test@example.com", Password = "Test123!", DisplayName = "TestUser" };
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                        .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                        .ReturnsAsync(IdentityResult.Success);

        var (succeeded, errors) = await _authService.RegisterAsync(request);

        Assert.True(succeeded);
        Assert.Empty(errors);
    }

    [Fact]
    public async Task RegisterAsync_ShouldReturnFailure_WhenCreationFails()
    {
        var errors = new List<IdentityError> { new IdentityError { Code = "PasswordTooShort", Description = "Password is too short." } };
        _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                        .ReturnsAsync(IdentityResult.Failed(errors.ToArray()));

        var request = new RegisterRequest { Email = "fail@example.com", Password = "123", DisplayName = "BadUser" };

        var (succeeded, returnedErrors) = await _authService.RegisterAsync(request);

        Assert.False(succeeded);
        Assert.Equal(errors, returnedErrors);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnTrue_WhenCredentialsAreValid()
    {
        var user = new ApplicationUser { Id = "1", Email = "user@example.com", DisplayName = "User" };

        _userManagerMock.Setup(um => um.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(user, It.IsAny<string>(), true, false))
                          .ReturnsAsync(SignInResult.Success);
        _userManagerMock.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

        var request = new LoginRequest { Email = user.Email, Password = "ValidPass123" };

        var response = await _authService.LoginAsync(request);

        Assert.True(response);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFalse_WhenUserNotFound()
    {
        _userManagerMock.Setup(um => um.FindByEmailAsync("unknown@example.com"))
                        .ReturnsAsync((ApplicationUser)null!);

        var request = new LoginRequest { Email = "unknown@example.com", Password = "123" };

        var result = await _authService.LoginAsync(request);

        Assert.False(result);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnFalse_WhenPasswordIsIncorrect()
    {
        var user = new ApplicationUser { Id = "1", Email = "user@example.com" };
        _userManagerMock.Setup(um => um.FindByEmailAsync(user.Email)).ReturnsAsync(user);
        _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(user, It.IsAny<string>(), true, false))
                          .ReturnsAsync(SignInResult.Failed);

        var request = new LoginRequest { Email = user.Email, Password = "WrongPass" };

        var result = await _authService.LoginAsync(request);

        Assert.False(result);
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
