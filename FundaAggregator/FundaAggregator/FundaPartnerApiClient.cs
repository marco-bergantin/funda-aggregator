using FundaAggregator.Model;
using Polly.Retry;
using Polly;
using System.Net.Http.Json;

namespace FundaAggregator;

public class FundaPartnerApiClient
{
    private const int DefaultPageSize = 25;

    private readonly ResiliencePipeline _retryPipeline;
    private readonly HttpClient _httpClient;
    private readonly Uri _baseApiUri;
    private readonly string _key;

    public FundaPartnerApiClient(HttpClient httpClient, Uri baseApiUri, string apiKey)
    {
        _httpClient = httpClient;
        _baseApiUri = baseApiUri;
        _key = apiKey;

        _retryPipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                Delay = TimeSpan.FromMilliseconds(50)
            })
            .Build();
    }

    public async Task<ApiResultObject?> GetResultsAsync(string type, 
        string searchQuery, int page = 1, int pageSize = DefaultPageSize)
    {
        var uri = new Uri($"{_baseApiUri}/json/{_key}/?type={type}&zo={searchQuery}&page={page}&pagesize={pageSize}");

        var response = await _retryPipeline.ExecuteAsync(async (cancellationToken) =>
        {
            var responseMessage = await _httpClient.GetAsync(uri, cancellationToken);
            responseMessage.EnsureSuccessStatusCode();
            return responseMessage;
        });

        return await response.Content.ReadFromJsonAsync<ApiResultObject>();
    }

    public async IAsyncEnumerable<Listing[]> GetAllResultsAsync(string type, string searchQuery)
    {
        var moreData = true;
        var currentPage = 1;

        while (moreData)
        {
            var pagedResults = await GetResultsAsync(type, searchQuery, currentPage);
            if (pagedResults?.Listings is null)
            {
                break;
            }

            yield return pagedResults.Listings;

            moreData = pagedResults.Paging.HuidigePagina < pagedResults.Paging.AantalPaginas;
            currentPage = pagedResults.Paging.HuidigePagina + 1;
        }
    }
}
