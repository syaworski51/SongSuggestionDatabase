using Microsoft.AspNetCore.Identity;

namespace SongSuggestionDatabase.Models.Users
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
