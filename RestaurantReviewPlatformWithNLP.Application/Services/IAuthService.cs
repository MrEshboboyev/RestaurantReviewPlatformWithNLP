using RestaurantReviewPlatformWithNLP.Application.Common.Models;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Application.Services
{
    public interface IAuthService
    {
        Task<ResponseDTO<string>> LoginAsync(LoginModel loginModel);
        Task<ResponseDTO<string>> RegisterAsync(RegisterModel registerModel);
        Task<ResponseDTO<string>> GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles);
    }
}
