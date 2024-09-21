using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Implementations
{
    public class LeaderboardService(IUnitOfWork unitOfWork,
        IMapper mapper,
        IRedisCacheService redisCacheService) : ILeaderboardService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;
        private readonly IRedisCacheService _redisCacheService = redisCacheService;

        public async Task<ResponseDTO<IEnumerable<LeaderboardWithRestaurantDTO>>> GetRanksWithRestaurantNameAsync()
        {
            try
            {
                // Try to get the leaderboard from Redis
                var leaderboardFromRedis = await _redisCacheService.GetAllLeaderboardsAsync();

                if (leaderboardFromRedis != null && leaderboardFromRedis.Any())
                {
                    var leaderboardDtos = leaderboardFromRedis
                        .Select((entry, index) => new LeaderboardWithRestaurantDTO
                        {
                            Rank = index + 1,
                            RestaurantName = entry.RestaurantName,
                            Score = entry.Score,
                            LastUpdated = entry.LastUpdated
                        })
                        .ToList();

                    return new ResponseDTO<IEnumerable<LeaderboardWithRestaurantDTO>>(leaderboardDtos);
                }

                // Fallback to the database if Redis cache is empty
                var leaderboardsFromDb = await _unitOfWork.Leaderboard.GetAllAsync(includeProperties: "Restaurant");

                if (!leaderboardsFromDb.Any())
                {
                    return new ResponseDTO<IEnumerable<LeaderboardWithRestaurantDTO>>(null, "No leaderboards found!");
                }

                var orderedLeaderboards = leaderboardsFromDb
                    .OrderByDescending(l => l.Score)
                    .Select((leaderboard, index) => new LeaderboardWithRestaurantDTO
                    {
                        Rank = index + 1,
                        RestaurantName = leaderboard.Restaurant.Name,
                        Score = leaderboard.Score,
                        LastUpdated = leaderboard.LastUpdated
                    })
                    .ToList();

                return new ResponseDTO<IEnumerable<LeaderboardWithRestaurantDTO>>(orderedLeaderboards);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<IEnumerable<LeaderboardWithRestaurantDTO>>(ex.Message);
            }
        }

        public async Task<ResponseDTO<List<LeaderboardDTO>>> GetTopRestaurantsAsync(int topCount)
        {
            try
            {
                // Attempt to fetch top restaurants from Redis
                var topRestaurantsFromRedis = await _redisCacheService.GetTopRestaurantsAsync(topCount);

                if (topRestaurantsFromRedis != null && topRestaurantsFromRedis.Any())
                {
                    return new ResponseDTO<List<LeaderboardDTO>>(_mapper.Map<List<LeaderboardDTO>>(topRestaurantsFromRedis));
                }

                // Fallback to the database if Redis does not have the data
                var restaurantsFromDb = await _unitOfWork.Restaurant.GetAllAsync(includeProperties: "Leaderboard");
                var topRestaurantsFromDb = restaurantsFromDb
                    .OrderByDescending(r => r.Leaderboard.Score)
                    .Take(topCount)
                    .ToList();

                return new ResponseDTO<List<LeaderboardDTO>>(
                    _mapper.Map<List<LeaderboardDTO>>(topRestaurantsFromDb.Select(r => r.Leaderboard)));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<LeaderboardDTO>>(ex.Message);
            }
        }

        public async Task<ResponseDTO<LeaderboardDTO>> GetLeaderboardByRestaurantAsync(Guid restaurantId)
        {
            try
            {
                // Try fetching leaderboard from Redis
                var leaderboardFromRedis = await _redisCacheService.GetLeaderboardByRestaurantAsync(restaurantId);

                if (leaderboardFromRedis != null)
                {
                    return new ResponseDTO<LeaderboardDTO>(_mapper.Map<LeaderboardDTO>(leaderboardFromRedis));
                }

                // Fallback to the database
                var leaderboardFromDb = await _unitOfWork.Leaderboard.GetAsync(
                    filter: l => l.RestaurantId.Equals(restaurantId),
                    includeProperties: "Restaurant")
                    ?? throw new Exception("Leaderboard not found!");

                return new ResponseDTO<LeaderboardDTO>(_mapper.Map<LeaderboardDTO>(leaderboardFromDb));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<LeaderboardDTO>(ex.Message);
            }
        }

        public async Task<ResponseDTO<bool>> UpdateAllLeaderboardsAsync()
        {
            try
            {
                var restaurants = await _unitOfWork.Restaurant.GetAllAsync(includeProperties: "Reviews");

                if (!restaurants.Any())
                    throw new Exception("No restaurants found!");

                foreach (var restaurant in restaurants)
                {
                    decimal averageRating = restaurant.Reviews.Any() ? restaurant.Reviews.Average(r => r.Rating) : 0;
                    float averageSentimentScore = restaurant.Reviews.Any() ? restaurant.Reviews.Average(r => r.SentimentScore) : 0;
                    decimal combinedScore = CalculateCombinedScore(averageRating, averageSentimentScore);
                    int rank = await CalculateRankAsync(restaurant.Id);

                    var leaderboard = await _unitOfWork.Leaderboard.GetAsync(l => l.RestaurantId == restaurant.Id);
                    if (leaderboard == null)
                    {
                        leaderboard = new Leaderboard
                        {
                            Id = Guid.NewGuid(),
                            RestaurantId = restaurant.Id,
                            Score = combinedScore,
                            Rank = rank,
                            LastUpdated = DateTime.UtcNow
                        };

                        await _unitOfWork.Leaderboard.AddAsync(leaderboard);
                    }
                    else
                    {
                        leaderboard.Score = combinedScore;
                        leaderboard.Rank = rank;
                        leaderboard.LastUpdated = DateTime.UtcNow;

                        await _unitOfWork.Leaderboard.UpdateAsync(leaderboard);
                    }

                    // Update Redis with the new leaderboard data
                    await _redisCacheService.UpdateLeaderboardAsync(restaurant.Id, restaurant.Name, combinedScore, rank);
                }

                await _unitOfWork.SaveAsync();

                return new ResponseDTO<bool>(true, "All leaderboards updated successfully!");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(ex.Message);
            }
        }

        #region Private methods

        private decimal CalculateCombinedScore(decimal rating, float sentimentScore)
        {
            // Formula to calculate the combined score, giving equal weight to both rating and sentiment score
            // You can adjust the weights if one factor is more important than the other
            return (decimal)(rating * 0.5M + (decimal)sentimentScore * 0.5M);
        }

        private async Task<int> CalculateRankAsync(Guid restaurantId)
        {
            var allLeaderboards = await _unitOfWork.Leaderboard.GetAllAsync(includeProperties: "Restaurant.Reviews");

            var orderedLeaderboards = allLeaderboards
                .OrderByDescending(l => CalculateCombinedScore(l.Restaurant.Reviews.Average(r => r.Rating), l.Restaurant.Reviews.Average(r => r.SentimentScore)))
                .ToList();

            int rank = orderedLeaderboards
                .Select((leaderboard, index) => new { leaderboard, index })
                .Where(l => l.leaderboard.RestaurantId == restaurantId)
                .Select(l => l.index + 1)
                .FirstOrDefault();

            return rank;
        }

        #endregion
    }
}
