namespace RestaurantReviewPlatformWithNLP.Application.DTOs
{
    public class LeaderboardWithRestaurantDTO
    {
        public int Rank { get; set; }
        public string RestaurantName { get; set; }
        public decimal Score { get; set; }
        public DateTime LastUpdated { get; set; }
    }

}
