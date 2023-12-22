using FundaAggregator.Model;
using Polly.Retry;
using Polly;
using System.Net.Http.Json;

namespace FundaAggregator
{
    public class FundaPartnerApiClient
    {
        private const string baseUrl = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json";

        private readonly ResiliencePipeline _retryPipeline;
        private readonly HttpClient _httpClient;
        private readonly string _key;

        public FundaPartnerApiClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _key = apiKey;

            _retryPipeline = new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    Delay = TimeSpan.FromMilliseconds(50)
                })
                .Build();
        }

        public async Task<ApiResultObject> GetResultsAsync(int page = 1, int pageSize = 25)
        {
            var uri = new Uri($"{baseUrl}/{_key}/?type=koop&zo=/amsterdam/tuin/&page={page}&pagesize={pageSize}");

            var response = await _retryPipeline.ExecuteAsync(async (cancellationToken) =>
            {
                var responseMessage = await _httpClient.GetAsync(uri, cancellationToken);
                responseMessage.EnsureSuccessStatusCode();
                return responseMessage;
            });

            var results = await response.Content.ReadFromJsonAsync<ApiResultObject>();

            return results;
        }

        public async Task<ApiResultObject> GetAllResultsAsync()
        {
            var allResults = new List<Model.Object>();
            var moreData = true;
            var currentPage = 1;

            while (moreData)
            {
                var pagedResults = await GetResultsAsync(currentPage);
                allResults.AddRange(pagedResults.Objects);

                moreData = pagedResults.Paging.HuidigePagina < pagedResults.Paging.AantalPaginas;
                currentPage = pagedResults.Paging.HuidigePagina + 1;
            }

            return new ApiResultObject { Objects = [.. allResults] };
        }
    }
}
