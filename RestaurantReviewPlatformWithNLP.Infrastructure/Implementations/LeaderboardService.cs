using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Implementations
{
    public class LeaderboardService(IUnitOfWork unitOfWork,
        IMapper mapper) : ILeaderboardService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDTO<IEnumerable<LeaderboardWithRestaurantDTO>>> GetRanksWithRestaurantNameAsync()
        {
            try
            {
                // Fetch all leaderboards, including the associated restaurant
                var leaderboards = await _unitOfWork.Leaderboard.GetAllAsync(includeProperties: "Restaurant");

                if (!leaderboards.Any())
                {
                    return new ResponseDTO<IEnumerable<LeaderboardWithRestaurantDTO>>(null, "No leaderboards found!");
                }

                // Order leaderboards by score in descending order and assign ranks
                var orderedLeaderboards = leaderboards
                    .OrderByDescending(l => l.Score)
                    .Select((leaderboard, index) => new LeaderboardWithRestaurantDTO
                    {
                        Rank = index + 1, // Rank starts at 1
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
                // getting all restaurants
                var restaurantsFromDb = await _unitOfWork.Restaurant.GetAllAsync(
                    includeProperties: "Leaderboard");

                // sorting and take count restaurant
                var topRestaurants = restaurantsFromDb
                    .OrderByDescending(r => r.Leaderboard.Rank)
                    .Take(topCount)
                    .ToList();

                return new ResponseDTO<List<LeaderboardDTO>>(
                    _mapper.Map<List<LeaderboardDTO>>(topRestaurants.Select(r => r.Leaderboard)));
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
                // get leaderboard
                var leaderboardFromDb = await _unitOfWork.Leaderboard.GetAsync(
                    filter: l => l.RestaurantId.Equals(restaurantId),
                    includeProperties: "Restaurant")
                    ?? throw new Exception("Leaderboard not found!");

                return new ResponseDTO<LeaderboardDTO>(
                    _mapper.Map<LeaderboardDTO>(leaderboardFromDb));
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
                // Fetch all restaurants along with their reviews
                var restaurants = await _unitOfWork.Restaurant.GetAllAsync(
                    includeProperties: "Reviews");

                if (restaurants == null || !restaurants.Any())
                    throw new Exception("No restaurants found!");

                // Iterate over each restaurant and update their leaderboard
                foreach (var restaurant in restaurants)
                {
                    // Calculate the average rating for each restaurant
                    decimal averageRating = restaurant.Reviews.Any()
                        ? restaurant.Reviews.Average(r => r.Rating)
                        : 0; // If no reviews, average rating is 0

                    // Use the private method to calculate the rank of the restaurant
                    int rank = await CalculateRankAsync(restaurant.Id);

                    // Fetch the leaderboard for the restaurant
                    var leaderboardFromDb = await _unitOfWork.Leaderboard.GetAsync(
                        filter: l => l.RestaurantId.Equals(restaurant.Id));

                    if (leaderboardFromDb == null)
                    {
                        // If no leaderboard exists, create a new one
                        leaderboardFromDb = new Leaderboard
                        {
                            Id = Guid.NewGuid(),
                            RestaurantId = restaurant.Id,
                            Score = averageRating,
                            Rank = rank,
                            LastUpdated = DateTime.UtcNow
                        };

                        await _unitOfWork.Leaderboard.AddAsync(leaderboardFromDb);
                    }
                    else
                    {
                        // If leaderboard exists, update it
                        leaderboardFromDb.Score = averageRating;
                        leaderboardFromDb.Rank = rank;
                        leaderboardFromDb.LastUpdated = DateTime.UtcNow;

                        await _unitOfWork.Leaderboard.UpdateAsync(leaderboardFromDb);
                    }
                }

                // Save all changes to the database
                await _unitOfWork.SaveAsync();

                return new ResponseDTO<bool>(true, "All leaderboards updated successfully!");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(ex.Message);
            }
        }

        #region Private methods
        private async Task<int> CalculateRankAsync(Guid restaurantId)
        {
            // Fetch all leaderboards from the database
            var allLeaderboards = await _unitOfWork.Leaderboard.GetAllAsync();

            // Order the leaderboards by Score in descending order and calculate the rank
            var orderedLeaderboards = allLeaderboards
                .OrderByDescending(l => l.Score)
                .ToList();

            // Find the rank of the given restaurant
            int rank = orderedLeaderboards
                .Select((leaderboard, index) => new { leaderboard, index })
                .Where(l => l.leaderboard.RestaurantId == restaurantId)
                .Select(l => l.index + 1) // Ranks start at 1
                .FirstOrDefault();

            return rank;
        }


        #endregion
    }
}
