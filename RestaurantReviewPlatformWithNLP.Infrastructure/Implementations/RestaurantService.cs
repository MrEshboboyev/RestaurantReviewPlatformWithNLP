﻿using AutoMapper;
using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;
using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Implementations
{
    public class RestaurantService(IUnitOfWork unitOfWork, 
        IMapper mapper) : IRestaurantService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IMapper _mapper = mapper;

        public async Task<ResponseDTO<IEnumerable<RestaurantDTO>>> GetAllRestaurantsAsync()
        {
            try
            {
                // getting all restaurants
                var restaurantsFromDb = await _unitOfWork.Restaurant.GetAllAsync(
                    includeProperties: "Reviews");
                
                // create restaurantDTOs list
                var restaurantDTOs = new List<RestaurantDTO>();

                foreach (var restaurant in restaurantsFromDb)
                {
                    // get restaurant reviews
                    var reviewsForRestaurant = await _unitOfWork.Review.GetAllAsync(
                        r => r.RestaurantId.Equals(restaurant.Id)
                        );

                    // calculate average rating
                    decimal averageRating = reviewsForRestaurant.Any() ? reviewsForRestaurant.Average(r => r.Rating) : 0;

                    // mapping and assigning
                    var restaurantDTO = _mapper.Map<RestaurantDTO>(restaurant);
                    restaurantDTO.AverageRating = averageRating;

                    restaurantDTOs.Add(restaurantDTO);
                }

                return new ResponseDTO<IEnumerable<RestaurantDTO>>(restaurantDTOs);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<IEnumerable<RestaurantDTO>>(ex.Message);
            }
        }

        public async Task<ResponseDTO<RestaurantDTO>> GetRestaurantByIdAsync(Guid restaurantId)
        {
            try
            {
                // getting restaurant
                var restaurantFromDb = await _unitOfWork.Restaurant.GetAsync(
                    filter: r => r.Id.Equals(restaurantId),
                    includeProperties: "Reviews")
                    ?? throw new Exception("Restaurant not found!");

                // get restaurant reviews
                var reviewsForRestaurant = await _unitOfWork.Review.GetAllAsync(
                    r => r.RestaurantId.Equals(restaurantFromDb.Id)
                    );

                // calculate average rating
                decimal averageRating = reviewsForRestaurant.Any() ? reviewsForRestaurant.Average(r => r.Rating) : 0;

                // mapping and assigning
                var restaurantDTO = _mapper.Map<RestaurantDTO>(restaurantFromDb);
                restaurantDTO.AverageRating = averageRating;

                return new ResponseDTO<RestaurantDTO>(restaurantDTO);
            }
            catch (Exception ex)
            {
                return new ResponseDTO<RestaurantDTO>(ex.Message);
            }
        }


        public async Task<ResponseDTO<RestaurantDTO>> CreateRestaurantAsync(RestaurantCreateDTO restaurantCreateDTO)
        {
            try
            {
                // mapping and add db
                var restaurantForDb = _mapper.Map<Restaurant>(restaurantCreateDTO);

                await _unitOfWork.Restaurant.AddAsync(restaurantForDb);
                await _unitOfWork.SaveAsync();

                // create leaderboard this new restaurant
                Leaderboard leaderboardForDb = new()
                {
                    LastUpdated = DateTime.UtcNow,
                    Rank = 0,
                    Score = 0,
                    RestaurantId = restaurantForDb.Id
                };

                // adding leaderboard to db and save
                await _unitOfWork.Leaderboard.AddAsync(leaderboardForDb);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO<RestaurantDTO>(_mapper.Map<RestaurantDTO>(restaurantForDb));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<RestaurantDTO>(ex.Message);
            }
        }

        public async Task<ResponseDTO<RestaurantDTO>> UpdateRestaurantAsync(Guid restaurantId, RestaurantUpdateDTO restaurantUpdateDTO)
        {
            try
            {
                // getting restaurant
                var restaurantFromDb = await _unitOfWork.Restaurant.GetAsync(r => r.Id.Equals(restaurantId))
                    ?? throw new Exception("Restaurant not found!");

                // mapping 
                _mapper.Map(restaurantUpdateDTO, restaurantFromDb);

                // update and save
                await _unitOfWork.Restaurant.UpdateAsync(restaurantFromDb);
                await _unitOfWork.SaveAsync();

                // update the leaderboard
                var leaderboardFromDb = await _unitOfWork.Leaderboard.GetAsync(
                    l => l.RestaurantId.Equals(restaurantId)) 
                    ?? throw new Exception("Leaderboard not found!");

                leaderboardFromDb.LastUpdated = DateTime.UtcNow;
                leaderboardFromDb.Score = restaurantFromDb.Reviews.Any()
                    ? restaurantFromDb.Reviews.Average(r => r.Rating)
                    : 0; // Recalculate score (average rating)

                await _unitOfWork.Leaderboard.UpdateAsync(leaderboardFromDb);
                await _unitOfWork.SaveAsync();

                return new ResponseDTO<RestaurantDTO>(_mapper.Map<RestaurantDTO>(restaurantFromDb));
            }
            catch (Exception ex)
            {
                return new ResponseDTO<RestaurantDTO>(ex.Message);
            }
        }

        public async Task<ResponseDTO<bool>> DeleteRestaurantAsync(Guid restaurantId)
        {
            try
            {
                // getting restaurant
                var restaurantFromDb = await _unitOfWork.Restaurant.GetAsync(r => r.Id.Equals(restaurantId))
                    ?? throw new Exception("Restaurant not found!");

                // update and save
                await _unitOfWork.Restaurant.RemoveAsync(restaurantFromDb);
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
