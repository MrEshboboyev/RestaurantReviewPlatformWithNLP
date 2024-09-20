using RestaurantReviewPlatformWithNLP.Application.DTOs;

namespace RestaurantReviewPlatformWithNLP.Application.Services.Interfaces
{
    public interface IRedisCacheService
    {
        Task<IEnumerable<LeaderboardDTO>> GetAllLeaderboardsAsync();
        Task<List<LeaderboardDTO>> GetTopRestaurantsAsync(int topCount);
        Task<LeaderboardDTO> GetLeaderboardByRestaurantAsync(Guid restaurantId);
        Task UpdateLeaderboardAsync(Guid restaurantId, decimal score, int rank);
    }
}
