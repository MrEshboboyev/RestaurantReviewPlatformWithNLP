namespace RestaurantReviewPlatformWithNLP.Application.DTOs
{
    public class ReviewDTO
    {
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public string UserId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
