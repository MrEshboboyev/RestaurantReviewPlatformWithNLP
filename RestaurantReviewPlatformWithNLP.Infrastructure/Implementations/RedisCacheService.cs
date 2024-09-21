using StackExchange.Redis;
using Newtonsoft.Json;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;

public class RedisCacheService : IRedisCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    private const string LeaderboardKey = "leaderboard";

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = _redis.GetDatabase();
    }

    public async Task<IEnumerable<LeaderboardDTO>> GetAllLeaderboardsAsync()
    {
        var sortedLeaderboards = await _db.SortedSetRangeByRankAsync(LeaderboardKey, order: Order.Descending);

        if (sortedLeaderboards.Length == 0)
            return Enumerable.Empty<LeaderboardDTO>();

        return sortedLeaderboards.Select(entry => JsonConvert.DeserializeObject<LeaderboardDTO>(entry));
    }

    public async Task<List<LeaderboardDTO>> GetTopRestaurantsAsync(int topCount)
    {
        var sortedLeaderboards = await _db.SortedSetRangeByRankAsync(LeaderboardKey, 0, topCount - 1, order: Order.Descending);

        if (sortedLeaderboards.Length == 0)
            return new List<LeaderboardDTO>();

        return sortedLeaderboards.Select(entry => JsonConvert.DeserializeObject<LeaderboardDTO>(entry)).ToList();
    }

    public async Task<LeaderboardDTO> GetLeaderboardByRestaurantAsync(Guid restaurantId)
    {
        var leaderboard = await _db.SortedSetScoreAsync(LeaderboardKey, restaurantId.ToString());

        if (leaderboard == null)
            return null;

        // Fetch the leaderboard by key and deserialize it
        var leaderboardJson = await _db.StringGetAsync(GenerateLeaderboardKey(restaurantId));
        return leaderboardJson.HasValue ? JsonConvert.DeserializeObject<LeaderboardDTO>(leaderboardJson) : null;
    }

    public async Task UpdateLeaderboardAsync(Guid restaurantId, decimal score, int rank)
    {
        // Store leaderboard in sorted set based on score
        await _db.SortedSetAddAsync(LeaderboardKey, restaurantId.ToString(), (double)score);

        // Create a LeaderboardDTO object and serialize it to store in Redis
        var leaderboardDto = new LeaderboardDTO
        {
            RestaurantId = restaurantId,
            Score = score,
            Rank = rank,
            LastUpdated = DateTime.UtcNow
        };

        var leaderboardJson = JsonConvert.SerializeObject(leaderboardDto);

        // Store the leaderboard JSON as a key-value pair with the restaurant ID as the key
        await _db.StringSetAsync(GenerateLeaderboardKey(restaurantId), leaderboardJson);
    }

    #region Private Helpers

    private string GenerateLeaderboardKey(Guid restaurantId)
    {
        return $"{LeaderboardKey}:{restaurantId}";
    }

    #endregion
}
