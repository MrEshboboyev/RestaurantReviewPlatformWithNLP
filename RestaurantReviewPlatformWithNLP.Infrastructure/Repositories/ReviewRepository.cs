using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Domain.Entities;
using RestaurantReviewPlatformWithNLP.Infrastructure.Data;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Repositories
{
    public class ReviewRepository(AppDbContext db) : Repository<Review>(db),
        IReviewRepository
    {
        private readonly AppDbContext _db = db;

        public async Task UpdateAsync(Review review)
        {
            _db.Reviews.Update(review);
        }
    }
}
