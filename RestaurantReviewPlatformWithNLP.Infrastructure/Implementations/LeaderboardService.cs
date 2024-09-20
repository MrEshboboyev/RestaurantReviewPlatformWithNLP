using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Implementations
{
    public class LeaderboardService(IUnitOfWork unitOfWork,
        IMapper mapper) : ILeaderboardService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

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
                    _mapper.Map<List<LeaderboardDTO>>(topRestaurants));
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
                    includeProperties: "Leaderboard")
                    ?? throw new Exception("Leaderboard not found!");

                return new ResponseDTO<LeaderboardDTO>(
                    _mapper.Map<LeaderboardDTO>(leaderboardFromDb));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<LeaderboardDTO>(ex.Message);
            }
        }

        public async Task<ResponseDTO<bool>> UpdateLeaderboardAsync(Guid restaurantId)
        {
            try
            {
                // Fetch the current restaurant and its reviews
                var restaurant = await _unitOfWork.Restaurant.GetAsync(
                    filter: r => r.Id.Equals(restaurantId),
                    includeProperties: "Reviews")
                    ?? throw new Exception("Restaurant not found!");

                // Calculate the average rating for the restaurant
                decimal averageRating = restaurant.Reviews.Any()
                    ? restaurant.Reviews.Average(r => r.Rating)
                    : 0; // If no reviews, average rating is 0

                // Use the private method to get the rank of the restaurant based on the average rating
                int rank = await CalculateRankAsync(restaurantId);

                // Get the leaderboard entry for the restaurant
                var leaderboardFromDb = await _unitOfWork.Leaderboard.GetAsync(
                    filter: l => l.RestaurantId.Equals(restaurantId))
                    ?? throw new Exception("Leaderboard not found!");

                // Update leaderboard
                leaderboardFromDb.LastUpdated = DateTime.UtcNow;
                leaderboardFromDb.Score = averageRating; // Save the average rating in leaderboard
                leaderboardFromDb.Rank = rank; // Save the calculated rank

                // Save changes
                await _unitOfWork.SaveAsync();

                return new ResponseDTO<bool>(true, "Leaderboard updated successfully!");
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(ex.Message);
            }
        }

        #region Private methods
        private async Task<int> CalculateRankAsync(Guid restaurantId)
        {
            // Fetch all restaurants and their reviews to calculate ranking
            var allRestaurants = await _unitOfWork.Restaurant.GetAllAsync(includeProperties: "Reviews");

            // Calculate the average rating for each restaurant and rank them
            var restaurantRankings = allRestaurants
                .Select(r => new
                {
                    RestaurantId = r.Id,
                    AverageRating = r.Reviews.Any() ? r.Reviews.Average(rv => rv.Rating) : 0
                })
                .OrderByDescending(r => r.AverageRating) // Higher rating = better rank
                .ToList();

            // Find the rank of the current restaurant
            int rank = restaurantRankings.FindIndex(r => r.RestaurantId == restaurantId) + 1; // Rank is 1-based

            return rank;
        }
        #endregion
    }
}
