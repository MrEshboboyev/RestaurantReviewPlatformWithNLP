namespace RestaurantReviewPlatformWithNLP.Application.DTOs
{
    public class ReviewUpdateDTO
    {
        public Guid RestaurantId { get; set; }
        public string UserId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
    }
}
