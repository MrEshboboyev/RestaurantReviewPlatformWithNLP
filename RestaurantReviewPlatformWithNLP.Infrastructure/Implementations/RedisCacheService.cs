using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using StackExchange.Redis;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Implementations
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;
        private const string LeaderboardKey = "restaurant_leaderboard";

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        // Adds or updates the score for a restaurant
        public async Task<ResponseDTO<bool>> SetLeaderboardScoreAsync(Guid restaurantId, decimal score)
        {
            try
            {
                bool result = await _database.SortedSetAddAsync(LeaderboardKey, restaurantId.ToString(), (double)score);
                return new ResponseDTO<bool>(result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>($"Failed to set leaderboard score: {ex.Message}");
            }
        }

        // Retrieves the score of a restaurant
        public async Task<ResponseDTO<double?>> GetLeaderboardScoreAsync(Guid restaurantId)
        {
            try
            {
                double? score = await _database.SortedSetScoreAsync(LeaderboardKey, restaurantId.ToString());
                return new ResponseDTO<double?>(score);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<double?>($"Failed to get leaderboard score: {ex.Message}");
            }
        }

        // Gets the top restaurants based on their scores
        public async Task<ResponseDTO<SortedSetEntry[]>> GetTopRestaurantsAsync(int count)
        {
            try
            {
                SortedSetEntry[] topRestaurants = await _database.SortedSetRangeByRankWithScoresAsync(LeaderboardKey, 0, count - 1, Order.Descending);
                return new ResponseDTO<SortedSetEntry[]>(topRestaurants);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<SortedSetEntry[]>($"Failed to get top restaurants: {ex.Message}");
            }
        }

        // Removes a restaurant from the leaderboard
        public async Task<ResponseDTO<bool>> RemoveRestaurantFromLeaderboardAsync(Guid restaurantId)
        {
            try
            {
                bool result = await _database.SortedSetRemoveAsync(LeaderboardKey, restaurantId.ToString());
                return new ResponseDTO<bool>(result);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>($"Failed to remove restaurant from leaderboard: {ex.Message}");
            }
        }
    }
}
