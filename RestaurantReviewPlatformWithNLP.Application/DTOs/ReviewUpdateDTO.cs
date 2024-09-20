namespace RestaurantReviewPlatformWithNLP.Application.DTOs
{
    public class ReviewUpdateDTO
    {
        public string UserId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
    }
}
