namespace RestaurantReviewPlatformWithNLP.Domain.Entities
{
    public class Restaurant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public List<Review> Reviews { get; set; }
        public Leaderboard Leaderboard { get; set; }
    }
}
