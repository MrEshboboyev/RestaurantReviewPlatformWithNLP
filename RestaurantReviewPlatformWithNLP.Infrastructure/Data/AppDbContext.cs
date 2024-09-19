using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : 
        IdentityDbContext<ApplicationUser>(options)
    {

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Leaderboard> Leaderboards { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // define relationships

            // Restaurant -> Review (One-to-Many)
            builder.Entity<Restaurant>()
                .HasMany(r => r.Reviews)
                .WithOne(r => r.Restaurant)
                .HasForeignKey(r => r.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> Review (One-to-Many)
            builder.Entity<ApplicationUser>()
                .HasMany(r => r.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            // One-to-One: Restaurant -> Leaderboard
            builder.Entity<Restaurant>()
                .HasOne(r => r.Leaderboard)
                .WithOne(l => l.Restaurant)
                .HasForeignKey<Leaderboard>(l => l.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
