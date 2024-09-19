using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RestaurantReviewPlatformWithNLP.Domain.Entities;
using RestaurantReviewPlatformWithNLP.Infrastructure.Data;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Configurations
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            // adding identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
