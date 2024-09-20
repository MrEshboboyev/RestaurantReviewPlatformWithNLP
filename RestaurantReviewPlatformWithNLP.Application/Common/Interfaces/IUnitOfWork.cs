namespace RestaurantReviewPlatformWithNLP.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        ILeaderboardRepository Leaderboard { get; }
        IReviewRepository Review { get; }
        IRestaurantRepository Restaurant { get; }

        Task SaveAsync();
    }
}
