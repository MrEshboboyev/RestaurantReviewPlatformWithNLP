namespace RestaurantReviewPlatformWithNLP.Application.Common.Models
{
    public class CreateReviewModel
    {
        public Guid RestaurantId { get; set; }
        public string ReviewText { get; set; }
        public decimal Rating { get; set; }
    }
}
