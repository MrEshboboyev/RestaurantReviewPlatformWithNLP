using Google.Cloud.Language.V1;
using RestaurantReviewPlatformWithNLP.Application.Services.Interfaces;

namespace RestaurantReviewPlatformWithNLP.Infrastructure.Implementations
{
    public class GoogleCloudNLPService : INLPService
    {
        private readonly LanguageServiceClient _languageServiceClient;

        public GoogleCloudNLPService()
        {
            _languageServiceClient = LanguageServiceClient.Create();
        }

        public async Task<float> AnalyzeSentimentAsync(string text)
        {
            try
            {
                var document = Document.FromPlainText(text);
                var response = await _languageServiceClient.AnalyzeSentimentAsync(document);
                return response.DocumentSentiment.Score;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
