using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using RestaurantReviewPlatformWithNLP.Infrastructure.Data;
using RestaurantReviewPlatformWithNLP.Infrastructure.Implementations;
using RestaurantReviewPlatformWithNLP.Infrastructure.Repositories;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Configurations
{
    public static class ServiceConfig
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // adding lifetimes
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ILeaderboardService, LeaderboardService>();
            services.AddScoped<IRestaurantService, RestaurantService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<INLPService, GoogleCloudNLPService>();

            // configure redis service
            services.AddSingleton(new RedisCacheService(configuration.GetSection("Redis")["ConnectionString"]));

            return services;
        }
    }
}
