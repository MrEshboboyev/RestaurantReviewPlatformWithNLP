using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Domain.Entities;
using RestaurantReviewPlatformWithNLP.Infrastructure.Data;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Repositories
{
    public class RestaurantRepository(AppDbContext db) : Repository<Restaurant>(db), IRestaurantRepository
    {
        private readonly AppDbContext _db = db;

        public async Task UpdateAsync(Restaurant restaurant)
        {
            _db.Update(restaurant);
        }
    }
}

