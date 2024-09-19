using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Application.Common.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task UpdateAsync(Review review);
    }
}
