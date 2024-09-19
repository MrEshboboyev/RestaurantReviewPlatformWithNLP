using Microsoft.AspNetCore.Identity;

namespace RestaurantReviewPlatformWithNLP.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        // Navigation Properties
        public ICollection<Review> Reviews { get; set; }
    }
}
