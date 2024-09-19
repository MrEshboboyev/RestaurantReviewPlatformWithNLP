namespace RestaurantReviewPlatformWithNLP.Domain.Entities
{
    public class Leaderboard
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public decimal Score { get; set; }
        public int Rank { get; set; }
        public DateTime LastUpdated { get; set; }

        // Navigation properties
        public Restaurant Restaurant { get; set; }
    }
}
