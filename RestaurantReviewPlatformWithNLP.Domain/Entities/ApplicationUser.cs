using Microsoft.AspNetCore.Identity;

namespace RestaurantReviewPlatformWithNLP.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
