using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SongSuggestionDatabase.Models.Users;
using SongSuggestionDatabase.Models;

namespace SongSuggestionDatabase.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Currency> Currencies { get; set; } = default!;
        public DbSet<Rating> Ratings { get; set; } = default!;
        public DbSet<Episode> Episodes { get; set; } = default!;
        public DbSet<Request> Requests { get; set; } = default!;
        public DbSet<BannedArtist> BannedList { get; set; } = default!;
        public DbSet<CatalogSortOption> CatalogSortOptions { get; set; } = default!;
    }
}