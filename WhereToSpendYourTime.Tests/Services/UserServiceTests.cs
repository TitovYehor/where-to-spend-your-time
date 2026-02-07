using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WhereToSpendYourTime.Api.Exceptions.Users;
using WhereToSpendYourTime.Api.Mapping;
using WhereToSpendYourTime.Api.Models.User;
using WhereToSpendYourTime.Api.Services.User;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Tests.Services;

public class UserServiceTests
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    private readonly UserService _service;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public UserServiceTests()
    {
        _userManagerMock = MockUserManager<ApplicationUser>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper();

        _service = new UserService(_db, _mapper, _userManagerMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ReturnsUsersWithRoles()
    {
        var role1 = new ApplicationRole { Id = "1", Name = "User" };
        var role2 = new ApplicationRole { Id = "2", Name = "Admin" };
        _db.Roles.AddRange(role1, role2);

        var user1 = new ApplicationUser { Id = "1", DisplayName = "Alice", Email = "a@test.com" };
        var user2 = new ApplicationUser { Id = "2", DisplayName = "Bob", Email = "b@test.com" };
        user1.UserRoles.Add(new ApplicationUserRole
        {
            User = user1,
            Role = role1,
            UserId = user1.Id,
            RoleId = role1.Id
        });
        user2.UserRoles.Add(new ApplicationUserRole
        {
            User = user2,
            Role = role2,
            UserId = user2.Id,
            RoleId = role2.Id
        });
        _db.Users.AddRange(user1, user2);
        await _db.SaveChangesAsync();

        var result = (await _service.GetAllUsersAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, u => u.DisplayName == "Alice" && u.Role == "User");
        Assert.Contains(result, u => u.DisplayName == "Bob" && u.Role == "Admin");
        Assert.Equal("Admin", result[0].Role);
    }


    [Fact]
    public async Task GetPagedUsersAsync_ReturnsPagedResults()
    {
        var role1 = new ApplicationRole { Id = "1", Name = "User" };
        var role2 = new ApplicationRole { Id = "2", Name = "Admin" };
        _db.Roles.AddRange(role1, role2);

        var user1 = new ApplicationUser
        {
            Id = "1",
            DisplayName = "Alice",
            Email = "alice@test.com",
        };
        var user2 = new ApplicationUser
        {
            Id = "2",
            DisplayName = "Bob",
            Email = "bob@test.com",
        };
        var user3 = new ApplicationUser
        {
            Id = "3",
            DisplayName = "Charlie",
            Email = "charlie@test.com",
        };

        user1.UserRoles.Add(new ApplicationUserRole
        {
            User = user1,
            Role = role1,
            UserId = user1.Id,
            RoleId = role1.Id
        });
        user2.UserRoles.Add(new ApplicationUserRole
        {
            User = user2,
            Role = role1,
            UserId = user2.Id,
            RoleId = role1.Id
        });
        user3.UserRoles.Add(new ApplicationUserRole
        {
            User = user3,
            Role = role2,
            UserId = user3.Id,
            RoleId = role2.Id
        });
        _db.Users.AddRange(user1, user2, user3);
        await _db.SaveChangesAsync();

        var filter = new UserFilterRequest { Page = 1, PageSize = 2 };

        var result = await _service.GetPagedUsersAsync(filter);

        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal("Charlie", result.Items[0].DisplayName);
    }

    [Fact]
    public async Task GetPagedUsersAsync_AppliesSearchFilter()
    {
        var role1 = new ApplicationRole { Id = "1", Name = "User" };
        _db.Roles.Add(role1);

        var user1 = new ApplicationUser
        {
            Id = "1",
            DisplayName = "Alpha",
            Email = "a@test.com",
        };
        var user2 = new ApplicationUser
        {
            Id = "2",
            DisplayName = "Beta",
            Email = "b@test.com",
        };

        user1.UserRoles.Add(new ApplicationUserRole
        {
            User = user1,
            Role = role1,
            UserId = user1.Id,
            RoleId = role1.Id
        });
        user2.UserRoles.Add(new ApplicationUserRole
        {
            User = user2,
            Role = role1,
            UserId = user2.Id,
            RoleId = role1.Id
        });
        _db.Users.AddRange(user1, user2);
        await _db.SaveChangesAsync();

        var filter = new UserFilterRequest { Page = 1, PageSize = 10, Search = "be" };

        var result = await _service.GetPagedUsersAsync(filter);

        Assert.Single(result.Items);
        Assert.Equal("Beta", result.Items[0].DisplayName);
    }

    [Fact]
    public async Task GetPagedUsersAsync_ReturnsEmpty_WhenNoMatches()
    {
        _db.Users.Add(new ApplicationUser { Id = "1", DisplayName = "UserOne", Email = "one@test.com" });
        await _db.SaveChangesAsync();

        var filter = new UserFilterRequest { Page = 1, PageSize = 10, Search = "zzz" };

        var result = await _service.GetPagedUsersAsync(filter);

        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task GetProfileAsync_ThrowsUserNotFoundException_WhenUserNotFound()
    {
        await Assert.ThrowsAsync<UserNotFoundException>(() =>
            _service.GetProfileAsync("nonexistent", true));
    }

    [Fact]
    public async Task GetProfileAsync_ReturnsProfile_WithEmail_WhenIsSelf()
    {
        var review1 = new Review
        {
            Id = 1,
            Title = "Review Title",
            Content = "Good content",
            Rating = 4,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            Item = new Item { Title = "Item1" }
        };
        var review2 = new Review
        {
            Id = 2,
            Title = "Another Review",
            Content = "More content",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Item = new Item { Title = "Item2" }
        };
        await _db.Reviews.AddRangeAsync(review1, review2);

        var comment = new Comment
        {
            Id = 1,
            Content = "Nice review!",
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Review = review2
        };
        await _db.Comments.AddAsync(comment);

        var user = new ApplicationUser
        {
            UserName = "user@example.com",
            Email = "user@example.com",
            DisplayName = "John Doe",
            Reviews = new List<Review> { review1 },
            Comments = new List<Comment> { comment }
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _service.GetProfileAsync(user.Id, true);

        Assert.Equal("user@example.com", result.Email);
        Assert.Single(result.Reviews);
        Assert.Single(result.Comments);
        Assert.Equal("John Doe", result.Reviews[0].Author);
        Assert.Equal("John Doe", result.Comments[0].Author);
    }

    [Fact]
    public async Task GetProfileAsync_HidesEmail_WhenNotSelf()
    {
        var user = new ApplicationUser
        {
            Id = "user2",
            Email = "private@example.com",
            DisplayName = "Jane",
            Reviews = new List<Review>(),
            Comments = new List<Comment>()
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var result = await _service.GetProfileAsync("user2", isSelf: false);

        Assert.Null(result.Email);
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsAllRolesOrdered()
    {
        _db.Roles.AddRange(
            new ApplicationRole { Id = "1", Name = "User" },
            new ApplicationRole { Id = "2", Name = "Admin" }
        );
        await _db.SaveChangesAsync();

        var result = (await _service.GetRolesAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(new[] { "Admin", "User" }, result);
    }

    [Fact]
    public async Task UpdateProfileAsync_UpdatesProfile_WhenValid()
    {
        var user = new ApplicationUser { Id = "user1", DisplayName = "Old Name" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        await _service.UpdateProfileAsync(user.Id, "New Name");

        Assert.Equal("New Name", user.DisplayName);
    }

    [Fact]
    public async Task UpdateProfileAsync_ThrowsUserNotFoundException_WhenUserNotFound()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("missing"))
            .ReturnsAsync((ApplicationUser?)null);

        await Assert.ThrowsAsync<UserNotFoundException>(
            () => _service.UpdateProfileAsync("missing", "New Name")
        );
    }

    [Fact]
    public async Task UpdateProfileAsync_ThrowsUserProfileUpdateFailedException_WhenUpdateFails()
    {
        var user = new ApplicationUser { Id = "user1", DisplayName = "Old Name" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed" }));

        var ex = await Assert.ThrowsAsync<UserProfileUpdateFailedException>(
            () => _service.UpdateProfileAsync(user.Id, "New Name")
        );

        Assert.Contains(ex.Errors, e => e.Description == "Failed");
    }

    [Fact]
    public async Task UpdateProfileAsync_ThrowsDemoAccountOperationForbiddenException_WhenDemoAccount()
    {
        var user = new ApplicationUser
        {
            Id = "1",
            Email = "demo@example.com"
        };

        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id))
            .ReturnsAsync(user);

        await Assert.ThrowsAsync<DemoAccountOperationForbiddenException>(
            () => _service.UpdateProfileAsync(user.Id, "Name")
        );
    }

    [Fact]
    public async Task ChangePasswordAsync_DoesNotThrow_WhenSuccess()
    {
        var user = new ApplicationUser { Id = "user1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, "old", "new"))
            .ReturnsAsync(IdentityResult.Success);

        var exception = await Record.ExceptionAsync(() =>
        _service.ChangePasswordAsync(user.Id, "old", "new"));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ChangePasswordAsync_ThrowsUserNotFoundException_WhenUserNotFound()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("missing"))
            .ReturnsAsync((ApplicationUser?)null);

        await Assert.ThrowsAsync<UserNotFoundException>(
            () => _service.ChangePasswordAsync("missing", "old", "new")
        );
    }

    [Fact]
    public async Task ChangePasswordAsync_ThrowsPasswordChangeFailedException_WhenChangeFails()
    {
        var user = new ApplicationUser { Id = "user1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id))
            .ReturnsAsync(user);
        _userManagerMock.Setup(m => m.ChangePasswordAsync(user, "old", "new"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Bad password" }));

        var ex = await Assert.ThrowsAsync<PasswordChangeFailedException>(
            () => _service.ChangePasswordAsync(user.Id, "old", "new")
        );

        Assert.Contains(ex.Errors, e => e.Description == "Bad password");
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ThrowsInvalidRoleException_WhenRoleIsEmpty()
    {
        await Assert.ThrowsAsync<InvalidRoleException>(
            () => _service.UpdateUserRoleAsync("1", "")
        );
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ThrowsInvalidRoleException_WhenRoleIsInvalid()
    {
        await Assert.ThrowsAsync<InvalidRoleException>(
            () => _service.UpdateUserRoleAsync("1", "SuperAdmin")
        );
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ThrowsUserNotFoundException_WhenUserNotFound()
    {
        await SeedRoleAsync("User");

        _userManagerMock.Setup(m => m.FindByIdAsync("missing")).ReturnsAsync((ApplicationUser?)null);

        await Assert.ThrowsAsync<UserNotFoundException>(
            () => _service.UpdateUserRoleAsync("missing", "User")
        );
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ThrowsUserRoleForbiddenException_WhenUserIsAdmin()
    {
        await SeedRoleAsync("User");

        var user = new ApplicationUser { Id = "1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

        await Assert.ThrowsAsync<UserRoleForbiddenException>(
            () => _service.UpdateUserRoleAsync("1", "User")
        );
    }

    [Fact]
    public async Task UpdateUserRoleAsync_RemovesExistingRolesAndAddsNew()
    {
        await SeedRoleAsync("Moderator");

        var user = new ApplicationUser { Id = "1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "OldRole" });
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(user, "Moderator"))
            .ReturnsAsync(IdentityResult.Success);

        await _service.UpdateUserRoleAsync(user.Id, "Moderator");

        _userManagerMock.Verify(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()), Times.Once);
        _userManagerMock.Verify(m => m.AddToRoleAsync(user, "Moderator"), Times.Once);
    }

    [Fact]
    public async Task UpdateUserRoleAsync_ThrowsUserRoleUpdateFailedException_WhenRemoveFails()
    {
        await SeedRoleAsync("User");

        var user = new ApplicationUser { Id = "1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "OldRole" });
        _userManagerMock.Setup(m => m.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "fail" }));

        var ex = await Assert.ThrowsAsync<UserRoleUpdateFailedException>(
            () => _service.UpdateUserRoleAsync(user.Id, "User")
        );

        Assert.Contains(ex.Errors, e => e.Description == "fail");
    }

    [Fact]
    public async Task DeleteUserAsync_ThrowsUserNotFoundException_WhenUserNotFound()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("missing")).ReturnsAsync((ApplicationUser?)null);

        await Assert.ThrowsAsync<UserNotFoundException>(
            () => _service.DeleteUserAsync("missing")
        );
    }

    [Fact]
    public async Task DeleteUserAsync_ThrowsUserDeleteForbiddenException_WhenUserIsAdmin()
    {
        var user = new ApplicationUser { Id = "1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

        await Assert.ThrowsAsync<UserDeleteForbiddenException>(
            () => _service.DeleteUserAsync("1")
        );
    }

    [Fact]
    public async Task DeleteUserAsync_DeletesUser_WhenNotAdmin()
    {
        var user = new ApplicationUser { Id = "1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
        _userManagerMock.Setup(m => m.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

        await _service.DeleteUserAsync(user.Id);
        _userManagerMock.Verify(m => m.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ThrowsUserDeleteFailedException_WhenDeleteFails()
    {
        var user = new ApplicationUser { Id = "1" };
        _userManagerMock.Setup(m => m.FindByIdAsync(user.Id)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);
        _userManagerMock.Setup(m => m.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "fail" }));

        var ex = await Assert.ThrowsAsync<UserDeleteFailedException>(
            () => _service.DeleteUserAsync(user.Id)
        );

        Assert.Contains(ex.Errors, e => e.Description == "fail");
    }

    private async Task SeedRoleAsync(string roleName)
    {
        _db.Roles.Add(new ApplicationRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = roleName
        });
        await _db.SaveChangesAsync();
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
}