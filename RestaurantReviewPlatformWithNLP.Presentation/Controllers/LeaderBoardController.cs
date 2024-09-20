using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;

namespace RestaurantReviewPlatformWithNLP.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderBoardController(ILeaderboardService leaderboardService) : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService = leaderboardService;

        [HttpGet("get-ranks-with-restaurant-name")]
        public async Task<IActionResult> GetRanksWithRestaurantName()
        {
            var result = await _leaderboardService.GetRanksWithRestaurantNameAsync();

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("get-top-restaurants")]
        public async Task<IActionResult> GetTopRestaurants(int count)
        {
            var result = await _leaderboardService.GetTopRestaurantsAsync(count);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("get-leaderboard-by-restaurant")]
        public async Task<IActionResult> GetLeaderboardByRestaurant(Guid restaurantId)
        {
            var result = await _leaderboardService.GetLeaderboardByRestaurantAsync(restaurantId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPut("update-leaderboards")]
        public async Task<IActionResult> UpdateLeaderboards()
        {
            var result = await _leaderboardService.UpdateAllLeaderboardsAsync();

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
