using AutoMapper;
using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Implementations
{
    public class ReviewService(IUnitOfWork unitOfWork, 
        IMapper mapper) : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        
        public async Task<ResponseDTO<List<ReviewDTO>>> GetReviewsByRestaurantAsync(Guid restaurantId)
        {
            try
            {
                // get review for restaurant
                var reviewsFromDb = await _unitOfWork.Review.GetAllAsync(
                    r => r.RestaurantId.Equals(restaurantId)
                    );

                return new ResponseDTO<List<ReviewDTO>>(
                    _mapper.Map<List<ReviewDTO>>(reviewsFromDb));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<List<ReviewDTO>>(ex.Message);
            }
        }

        public async Task<ResponseDTO<ReviewDTO>> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO)
        {
            try
            {
                // mapping review
                var reviewForDb = _mapper.Map<Review>(reviewCreateDTO);

                // create review and save
                await _unitOfWork.Review.AddAsync(reviewForDb);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO<ReviewDTO>(
                    _mapper.Map<ReviewDTO>(reviewForDb));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReviewDTO>(ex.Message);
            }
        }

        public async Task<ResponseDTO<ReviewDTO>> UpdateReviewAsync(Guid reviewId, ReviewUpdateDTO reviewUpdateDTO)
        {
            try
            {
                // get review for restaurant
                var reviewFromDb = await _unitOfWork.Review.GetAsync(
                    r => r.Id.Equals(reviewId) && r.UserId.Equals(reviewUpdateDTO.UserId)
                    ) ?? throw new Exception("Review not found!");

                // mapping review
                _mapper.Map(reviewUpdateDTO, reviewFromDb);

                // update review and save
                await _unitOfWork.Review.UpdateAsync(reviewFromDb);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO<ReviewDTO>(
                    _mapper.Map<ReviewDTO>(reviewFromDb));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<ReviewDTO>(ex.Message);
            }
        }

        public async Task<ResponseDTO<bool>> DeleteReviewAsync(ReviewDeleteDTO reviewDeleteDTO)
        {
            try
            {
                // get review for restaurant
                var reviewFromDb = await _unitOfWork.Review.GetAsync(
                    r => r.Id.Equals(reviewDeleteDTO.ReviewId) && 
                    r.UserId.Equals(reviewDeleteDTO.UserId)
                    ) ?? throw new Exception("Review not found!");

                // remove review and save
                await _unitOfWork.Review.RemoveAsync(reviewFromDb);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO<bool>(true);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<bool>(ex.Message);
            }
        }
    }
}
