using RestaurantReviewPlatformWithNLP.Application.Common.Models;
using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Application.Services
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginModel loginModel);
        Task RegisterAsync(RegisterModel registerModel);
        Task<string> GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles);
    }
}
