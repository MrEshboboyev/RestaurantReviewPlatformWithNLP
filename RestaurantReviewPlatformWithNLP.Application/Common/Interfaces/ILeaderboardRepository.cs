using RestaurantReviewPlatformWithNLP.Domain.Entities;

namespace RestaurantReviewPlatformWithNLP.Application.Common.Interfaces
{
    public interface ILeaderboardRepository : IRepository<Leaderboard>
    {
        Task UpdateAsync(Leaderboard leaderboard);
    }
}
