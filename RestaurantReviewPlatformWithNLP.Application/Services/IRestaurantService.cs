using RestaurantReviewPlatformWithNLP.Application.DTOs;
using System.ComponentModel;

namespace RestaurantReviewPlatformWithNLP.Application.Services
{
    public interface IRestaurantService
    {
        Task<ResponseDTO<IEnumerable<RestaurantDTO>>> GetAllRestaurantsAsync();
        Task<ResponseDTO<RestaurantDTO>> GetRestaurantByIdAsync(Guid restaurantId);
        Task<ResponseDTO<RestaurantDTO>> CreateRestaurantAsync(RestaurantCreateDTO restaurantCreateDTO);
        Task<ResponseDTO<RestaurantDTO>> UpdateRestaurantAsync(Guid restaurantId, RestaurantUpdateDTO restaurantUpdateDTO);
        Task<ResponseDTO<bool>> DeleteRestaurantAsync(Guid restaurantId);
    }
}
