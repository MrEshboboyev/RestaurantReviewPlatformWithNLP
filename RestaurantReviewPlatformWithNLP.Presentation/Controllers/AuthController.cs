using Microsoft.AspNetCore.Mvc;
using RestaurantReviewPlatformWithNLP.Application.Common.Models;
using RestaurantReviewPlatformWithNLP.Application.Services;

namespace RestaurantReviewPlatformWithNLP.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // inject Auth Service
        private readonly IAuthService _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.LoginAsync(loginModel);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _authService.RegisterAsync(registerModel);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
