namespace RestaurantReviewPlatformWithNLP.Application.DTOs
{
    public class RestaurantDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        // Dynamically calculated, not stored in the database
        public decimal AverageRating { get; set; }

        public List<ReviewDTO> Reviews { get; set; }
    }
}
