using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Services
{
    public static class DbInitializerService
    {
        public static void SeedDatabase(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            dbInitializer.Initialize();
        }
    }
}
