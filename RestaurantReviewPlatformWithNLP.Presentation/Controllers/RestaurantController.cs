using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantReviewPlatformWithNLP.Application.Common.Utility;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;

namespace RestaurantReviewPlatformWithNLP.Presentation.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController(IRestaurantService restaurantService
        ) : ControllerBase
    {
        private readonly IRestaurantService _restaurantService = restaurantService;

        [AllowAnonymous]
        [HttpGet("get-all-restaurants")]
        public async Task<IActionResult> GetAllRestaurants()
        {
            var result = await _restaurantService.GetAllRestaurantsAsync();

            if (!result.Success) 
                return BadRequest(result.Message);

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("get-restaurant-by-id")]
        public async Task<IActionResult> GetRestaurantById(Guid restaurantId)
        {
            var result = await _restaurantService.GetRestaurantByIdAsync(restaurantId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("create-restaurant")]
        public async Task<IActionResult> CreateRestaurant([FromBody] RestaurantCreateDTO restaurantCreateDTO)
        {
            var result = await _restaurantService.CreateRestaurantAsync(restaurantCreateDTO);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPut("update-restaurant")]
        public async Task<IActionResult> UpdateRestaurant(Guid restaurantId, 
            [FromBody] RestaurantUpdateDTO restaurantUpdateDTO)
        {
            var result = await _restaurantService.UpdateRestaurantAsync(restaurantId, restaurantUpdateDTO);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
        
        [HttpDelete("delete-restaurant")]
        public async Task<IActionResult> DeleteRestaurant(Guid restaurantId)
        {
            var result = await _restaurantService.DeleteRestaurantAsync(restaurantId);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
