namespace RestaurantReviewPlatformWithNLP.Application.Services.Interfaces
{
    public interface INLPService
    {
        Task<float> AnalyzeSentimentAsync(string text);
    }
}
