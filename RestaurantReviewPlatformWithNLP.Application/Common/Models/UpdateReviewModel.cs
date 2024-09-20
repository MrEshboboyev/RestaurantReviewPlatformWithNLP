namespace RestaurantReviewPlatformWithNLP.Application.Common.Models
{
    public class UpdateReviewModel
    {
        public Guid RestaurantId { get; set; }
        public string UserId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
    }
}
