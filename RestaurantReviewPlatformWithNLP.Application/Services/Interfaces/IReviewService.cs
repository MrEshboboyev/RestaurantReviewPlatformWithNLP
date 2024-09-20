using RestaurantReviewPlatformWithNLP.Application.DTOs;

namespace RestaurantReviewPlatformWithNLP.Application.Services.Interfaces
{
    public interface IReviewService
    {
        Task<ResponseDTO<List<ReviewDTO>>> GetReviewsByRestaurantAsync(Guid restaurantId);
        Task<ResponseDTO<ReviewDTO>> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO);
        Task<ResponseDTO<ReviewDTO>> UpdateReviewAsync(Guid reviewId, ReviewUpdateDTO reviewUpdateDTO);
        Task<ResponseDTO<bool>> DeleteReviewAsync(ReviewDeleteDTO reviewDeleteDTO);
    }
}
