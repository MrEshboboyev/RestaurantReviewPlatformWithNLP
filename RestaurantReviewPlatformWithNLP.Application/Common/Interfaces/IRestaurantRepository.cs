using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Application.Common.Interfaces
{
    public interface IRestaurantRepository : IRepository<Restaurant>
    {
        Task UpdateAsync(Restaurant restaurant);
    }
}
