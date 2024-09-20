namespace RestaurantReviewPlatformWithNLP.Application.DTOs
{
    public class ReviewCreateDTO
    {
        public Guid RestaurantId { get; set; }
        public string UserId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
    }
}
