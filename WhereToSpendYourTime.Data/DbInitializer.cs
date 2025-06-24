using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    { 
        using var scope = services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();

        await SeedRoles(roleManager);

        var users = await SeedUsers(userManager);

        await SeedCategories(db);
        await SeedItems(db);
        await SeedReviewsAndComments(db, users);

        await db.SaveChangesAsync();
    }

    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Moderator", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task<Dictionary<string, ApplicationUser>> SeedUsers(UserManager<ApplicationUser> userManager)
    {
        var result = new Dictionary<string, ApplicationUser>();

        if (await userManager.FindByEmailAsync("admin@example.com") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
                DisplayName = "Admin"
            };

            await userManager.CreateAsync(admin, "Admin123!");
            await userManager.AddToRoleAsync(admin, "Admin");
            result.Add("admin", admin);
        }

        if (await userManager.FindByEmailAsync("user1@example.com") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "user1@example.com",
                Email = "user1@example.com",
                DisplayName = "John Doe"
            };

            await userManager.CreateAsync(user, "User123!");
            await userManager.AddToRoleAsync(user, "User");
            result["user1"] = user;
        }

        if (await userManager.FindByEmailAsync("user2@example.com") == null)
        {
            var user = new ApplicationUser
            {
                UserName = "user2@example.com",
                Email = "user2@example.com",
                DisplayName = "Greg Bor"
            };

            await userManager.CreateAsync(user, "User123!");
            await userManager.AddToRoleAsync(user, "User");
            result["user2"] = user;
        }

        return result;
    }

    private static async Task SeedCategories(AppDbContext db)
    {
        if (await db.Categories.AnyAsync())
        {
            return;
        }

        db.Categories.AddRange(
            new Category { Name = "Games" },
            new Category { Name = "Movies" },
            new Category { Name = "Books" }
        );

        await db.SaveChangesAsync();
    }

    private static async Task SeedItems(AppDbContext db)
    {
        if (await db.Items.AnyAsync())
        {
            return;
        }

        var categories = await db.Categories.ToListAsync();

        var games = categories.First(c => c.Name == "Games");
        var movies = categories.First(c => c.Name == "Movies");
        var books = categories.First(c => c.Name == "Books");

        db.Items.AddRange(
            new Item { 
                Title = "The Witcher 3", 
                Description = "Open-world, action RPG where players control Geralt of Rivia," +
                    " a monster hunter, on a quest to find his adopted daughter, Ciri",
                CategoryId = games.Id 
            },
            new Item {  
                Title = "The Witcher 2", 
                Description = "Action RPG sequel to The Witcher, where players control Geralt of Rivia, a monster hunter, " +
                    "as he navigates a world of political intrigue and conspiracy following an attempt on King Foltest's life",
                CategoryId = games.Id 
            },
            new Item {
                Title = "Factorio", 
                Description = "A construction and management simulation game where players build and automate" +
                    " factories to produce increasingly complex items, ultimately launching a rocket into space", 
                CategoryId = games.Id 
            },

            new Item { 
                Title = "Avatar", 
                Description = "Science-fiction film, which tells the story of Jake Sully, a paraplegic Marine, " +
                    "who is sent to the alien world of Pandora", 
                CategoryId = movies.Id 
            },
            new Item
            {
                Title = "The Avengers",
                Description = "A superhero film where Earth's mightiest heroes unite to stop Loki " +
                    "and his alien army from enslaving humanity",
                CategoryId = movies.Id
            },
            new Item
            {
                Title = "Django Unchained",
                Description = "Western film, which tells the story of Django, a freed slave, who teams up with a German bounty hunter" +
                    " named Dr. King Schultz to rescue Django's wife, Broomhilda, from a cruel plantation owner named Calvin Candie",
                CategoryId = movies.Id
            },

            new Item
            {
                Title = "The Lord of the Rings",
                Description = "Epic high fantasy novel about the war of the peoples of the fantasy world" +
                    " Middle-earth against a dark lord known as 'Sauron'",
                CategoryId = books.Id
            },
            new Item
            {
                Title = "The Adventures of Sherlock Holmes",
                Description = "A collection of twelve short stories, which introduce the brilliant, eccentric detective Sherlock Holmes" +
                    " and his companion, Dr. John Watson, as they solve various mysteries in London",
                CategoryId = books.Id
            },
            new Item
            {
                Title = "The Witcher",
                Description = "a collection of fantasy novels and short stories centered around Geralt of Rivia," +
                    " a monster hunter known as a Witcher",
                CategoryId = books.Id
            }
        );

        await db.SaveChangesAsync();
    }

    private static async Task SeedReviewsAndComments(AppDbContext db, Dictionary<string, ApplicationUser> users)
    {
        if (await db.Reviews.AnyAsync())
        {
            return;
        }

        var user1 = users["user1"];
        var user2 = users["user2"];
        var admin = users["admin"];

        var witcher3 = await db.Items.FirstAsync(i => i.Title == "The Witcher 3");
        var witcher2 = await db.Items.FirstAsync(i => i.Title == "The Witcher 2");
        var factorio = await db.Items.FirstAsync(i => i.Title == "Factorio");

        var avatar = await db.Items.FirstAsync(i => i.Title == "Avatar");
        var django = await db.Items.FirstAsync(i => i.Title == "Django Unchained");

        var lotr = await db.Items.FirstAsync(i => i.Title == "The Lord of the Rings");
        var sherlock = await db.Items.FirstAsync(i => i.Title == "The Adventures of Sherlock Holmes");

        var reviewGame1 = new Review
        {
            Title = "Amazing Game",
            Content = "The world, quests, and story are all amazing",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UserId = user1.Id,
            ItemId = witcher3.Id
        };
        var reviewGame2 = new Review
        {
            Title = "Cool Game",
            Content = "Interesting craft system",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UserId = user1.Id,
            ItemId = factorio.Id
        };
        var reviewGame3 = new Review
        {
            Title = "Good RPG",
            Content = "Cool quests, awesome story",
            Rating = 4,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UserId = user2.Id,
            ItemId = witcher2.Id
        };
        var reviewGame4 = new Review
        {
            Title = "Interesting factory builder",
            Content = "Very addictive at first half of the game but may be a bit tedious in later stages",
            Rating = 3,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UserId = admin.Id,
            ItemId = factorio.Id
        };

        var reviewMovie1 = new Review
        {
            Title = "Fascinating fantasy movie",
            Content = "Awesome graphic, cool story",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UserId = user1.Id,
            ItemId = avatar.Id
        };
        var reviewMovie2 = new Review
        {
            Title = "Cool western movie",
            Content = "Fascinating story in western style",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UserId = user2.Id,
            ItemId = django.Id
        };

        var reviewBook1 = new Review
        {
            Title = "Incredible book wit a lot of world-building",
            Content = "Cool story, interesting world descriptions",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UserId = user1.Id,
            ItemId = lotr.Id
        };
        var reviewBook2 = new Review
        {
            Title = "Cool slavic fantasy book",
            Content = "Fascinating plot, great world-building",
            Rating = 5,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UserId = user2.Id,
            ItemId = sherlock.Id
        };

        db.Reviews.AddRange(reviewGame1, reviewGame2, reviewGame3, reviewGame4, reviewMovie1, reviewMovie2, reviewBook1, reviewBook2);

        db.Comments.AddRange(
            new Comment
            {
                Content = "I totally agree!",
                CreatedAt = DateTime.UtcNow,
                Review = reviewGame1,
                UserId = user2.Id
            },
            new Comment
            {
                Content = "One of my favorites!",
                CreatedAt = DateTime.UtcNow,
                Review = reviewGame2,
                UserId = user2.Id
            },
            new Comment
            {
                Content = "Nice catch!",
                CreatedAt = DateTime.UtcNow,
                Review = reviewGame3,
                UserId = user1.Id
            },

            new Comment
            {
                Content = "Awesome film!",
                CreatedAt = DateTime.UtcNow,
                Review = reviewMovie1,
                UserId = user2.Id
            },

            new Comment
            {
                Content = "The masterpiece!",
                CreatedAt = DateTime.UtcNow,
                Review = reviewBook2,
                UserId = user1.Id
            }
        );

        await db.SaveChangesAsync();
    }
}
