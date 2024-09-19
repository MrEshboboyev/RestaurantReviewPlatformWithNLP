using RestaurantReviewPlatformWithNLP.Application.Common.Interfaces;
using RestaurantReviewPlatformWithNLP.Domain.Entities;
using RestaurantReviewPlatformWithNLP.Infrastructure.Data;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Repositories
{
    public class LeaderboardRepository(AppDbContext db) : Repository<Leaderboard>(db),
        ILeaderboardRepository
    {
        private readonly AppDbContext _db = db;

        public async Task UpdateAsync(Leaderboard leaderboard)
        {
            _db.Leaderboards.Update(leaderboard);
        }
    }
}
