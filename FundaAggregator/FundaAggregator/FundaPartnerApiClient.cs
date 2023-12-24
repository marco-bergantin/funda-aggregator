using FundaAggregator.Model;
using Polly.Retry;
using Polly;
using System.Net.Http.Json;

namespace FundaAggregator;

public class FundaPartnerApiClient(HttpClient httpClient,
    Uri baseApiUri, string apiKey, RetryStrategyOptions retryOptions)
{
    private const int DefaultPageSize = 25;

    private readonly HttpClient _httpClient = httpClient;
    private readonly Uri _baseApiUri = baseApiUri;
    private readonly string _key = apiKey;

    private readonly ResiliencePipeline _retryPipeline = new ResiliencePipelineBuilder()
        .AddRetry(retryOptions)
        .Build();

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

            if (pagedResults.Paging is null)
            {
                break;
            }

            moreData = pagedResults.Paging.HuidigePagina < pagedResults.Paging.AantalPaginas;
            currentPage = pagedResults.Paging.HuidigePagina + 1;
        }
    }
}
