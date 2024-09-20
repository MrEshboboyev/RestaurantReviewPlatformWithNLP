using RestaurantReviewPlatformWithNLP.Application.DTOs;

namespace RestaurantReviewPlatformWithNLP.Application.Services.Interfaces
{
    public interface ILeaderboardService
    {
        Task<ResponseDTO<List<LeaderboardDTO>>> GetTopRestaurantsAsync(int topCount);
        Task<ResponseDTO<LeaderboardDTO>> GetLeaderboardByRestaurantAsync(Guid restaurantId);
        Task<ResponseDTO<bool>> UpdateLeaderboardAsync(Guid restaurantId);
    }
}
