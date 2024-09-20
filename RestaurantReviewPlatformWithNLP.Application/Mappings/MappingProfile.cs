using AutoMapper;
using RestaurantReviewPlatformWithNLP.Application.DTOs;
using RestaurantReviewPlatformWithNLP.Application.Services;
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
        }
    }
}
