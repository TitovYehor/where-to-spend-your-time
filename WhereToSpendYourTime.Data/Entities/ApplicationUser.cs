using Microsoft.AspNetCore.Identity;

namespace WhereToSpendYourTime.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
