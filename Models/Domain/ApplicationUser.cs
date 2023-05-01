using Microsoft.AspNetCore.Identity;

namespace DOT6_IdentityWeb.Models.Domain
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        public string? ProfilePicture { get; set; } 
    }
}
