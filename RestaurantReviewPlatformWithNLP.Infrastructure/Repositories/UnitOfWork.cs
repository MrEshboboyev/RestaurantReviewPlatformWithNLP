using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Infrastructure.Data;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Repositories
{
    public class UnitOfWork(AppDbContext db) : IUnitOfWork
    {

        public ILeaderboardRepository Leaderboard { get; private set; } = new LeaderboardRepository(db);
        public IReviewRepository Review { get; private set; } = new ReviewRepository(db);
        public IRestaurantRepository Restaurant { get; private set; } = new RestaurantRepository(db);

        private readonly AppDbContext _db = db;

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
