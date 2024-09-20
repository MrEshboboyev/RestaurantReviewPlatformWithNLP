using RestaurantReviewPlatformWithNLP.Application.DTOs;
using StackExchange.Redis;

namespace RestaurantReviewPlatformWithNLP.Application.Services.Interfaces
{
    public interface IRedisCacheService
    {
        Task<ResponseDTO<bool>> SetLeaderboardScoreAsync(Guid restaurantId, decimal score);
        Task<ResponseDTO<double?>> GetLeaderboardScoreAsync(Guid restaurantId);
        Task<ResponseDTO<SortedSetEntry[]>> GetTopRestaurantsAsync(int count);
        Task<ResponseDTO<bool>> RemoveRestaurantFromLeaderboardAsync(Guid restaurantId);
    }
}
