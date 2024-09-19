namespace RestaurantReviewPlatformWithNLP.Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public string UserId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation Property
        public ApplicationUser User { get; set; }
        public Restaurant Restaurant { get; set; }
    }
}
