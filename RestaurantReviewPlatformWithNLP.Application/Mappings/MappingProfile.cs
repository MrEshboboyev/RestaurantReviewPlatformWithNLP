using AutoMapper;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            #region Restaurant

            // Restaurant -> RestaurantDTO
            CreateMap<Restaurant, RestaurantDTO>();

            // RestaurantCreateDTO -> Restaurant
            CreateMap<RestaurantCreateDTO, Restaurant>();

            // RestaurantUpdateDTO -> Restaurant
            CreateMap<RestaurantUpdateDTO, Restaurant>();

            #endregion

            #region Review

            // Review -> ReviewDTO
            CreateMap<Review, ReviewDTO>();

            // ReviewCreateDTO -> Review
            CreateMap<ReviewCreateDTO, Review>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            // ReviewUpdateDTO -> Review
            CreateMap<ReviewUpdateDTO, Review>();

            #endregion

            #region Leaderboard

            // Leaderboard -> LeaderboardDTO
            CreateMap<Leaderboard, LeaderboardDTO>()
                .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.Name));
            #endregion
        }
    }
}
