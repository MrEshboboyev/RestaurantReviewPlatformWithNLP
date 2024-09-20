using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using RestaurantReviewPlatformWithNLP.Infrastructure.Data;
using RestaurantReviewPlatformWithNLP.Infrastructure.Implementations;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Configurations
{
    public static class ServiceConfig
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // adding lifetimes
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
