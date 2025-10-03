using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WhereToSpendYourTime.Api.Handlers;
using WhereToSpendYourTime.Api.Helpers;
using WhereToSpendYourTime.Api.Services.Auth;
using WhereToSpendYourTime.Api.Services.Category;
using WhereToSpendYourTime.Api.Services.Comment;
using WhereToSpendYourTime.Api.Services.Item;
using WhereToSpendYourTime.Api.Services.Media;
using WhereToSpendYourTime.Api.Services.Review;
using WhereToSpendYourTime.Api.Services.Stats;
using WhereToSpendYourTime.Api.Services.Tags;
using WhereToSpendYourTime.Api.Services.User;
using WhereToSpendYourTime.Data;
using WhereToSpendYourTime.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

string? connStr = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                 ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrWhiteSpace(connStr) && connStr.StartsWith("postgresql://"))
{
    connStr = PostgreUrlConverter.ConvertPostgresUrlToConnectionString(connStr);
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connStr));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddScoped(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var connectionString = config["Azure:BlobConnectionString"];
    var containerName = config["Azure:BlobContainerName"];

    return new BlobContainerClient(connectionString, containerName);
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMediaService, MediaService>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/api/auth/login";
    options.AccessDeniedPath = "/api/auth/denied";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://where-to-spend-your-time.onrender.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseRouting();

app.UseCors("AllowFrontend");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedAsync(services);
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

await app.RunAsync();