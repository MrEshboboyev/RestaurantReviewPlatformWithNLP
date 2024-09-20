namespace RestaurantReviewPlatformWithNLP.Application.DTOs
{
    public class LeaderboardDTO
    {
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public decimal Score { get; set; }
        public int Rank { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
