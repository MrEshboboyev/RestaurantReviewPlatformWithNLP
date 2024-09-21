using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using StackExchange.Redis;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Configurations
{
    public static class RedisConfig
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, 
            IConfiguration configuration)
        {
            var redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"] ?? "defaultString");
            services.AddSingleton<IConnectionMultiplexer>(redis);
            services.AddScoped<IRedisCacheService, RedisCacheService>();
            return services;
        }
    }
}
