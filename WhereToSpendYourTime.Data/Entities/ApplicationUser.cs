using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhereToSpendYourTime.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Comment> Comment { get; set; } = new List<Comment>();
}
