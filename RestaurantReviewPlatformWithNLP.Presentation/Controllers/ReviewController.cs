using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestaurantReviewPlatformWithNLP.Application.Common.Models;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace RestaurantReviewPlatformWithNLP.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController(IReviewService reviewService) : ControllerBase
    {
        private readonly IReviewService _reviewService = reviewService;

        #region Private methods
        private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new Exception("Login Required!");
        #endregion

        [HttpGet("get-reviews-by-restaurant")]
        public async Task<IActionResult> GetReviewsByRestaurant(Guid restaurantId)
        {
            var result = await _reviewService.GetReviewsByRestaurantAsync(restaurantId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("create-review")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewModel createReviewModel)
        {
            // prepare dto
            ReviewCreateDTO reviewCreateDTO = new()
            {
                UserId = GetUserId(),
                Rating = createReviewModel.Rating,
                RestaurantId = createReviewModel.RestaurantId,
                ReviewText = createReviewModel.ReviewText
            };

            var result = await _reviewService.CreateReviewAsync(reviewCreateDTO);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("update-review")]
        public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewModel updateReviewModel)
        {
            // prepare dto
            ReviewUpdateDTO reviewUpdateDTO = new()
            {
                UserId = GetUserId(),
                Rating = updateReviewModel.Rating,
                RestaurantId = updateReviewModel.RestaurantId,
                ReviewText = updateReviewModel.ReviewText
            };

            var result = await _reviewService.UpdateReviewAsync(reviewId, reviewUpdateDTO);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("delete-review")]
        public async Task<IActionResult> DeleteReview(Guid reviewId)
        {
            var result = await _reviewService.DeleteReviewAsync(reviewId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
