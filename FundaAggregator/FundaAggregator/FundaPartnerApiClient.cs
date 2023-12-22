using FundaAggregator.Model;
using System.Net.Http.Json;

namespace FundaAggregator
{
    internal class FundaPartnerApiClient
    {
        private static string baseUrl = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json";

        private HttpClient _httpClient;
        private string _key;

        public FundaPartnerApiClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _key = apiKey;
        }

        public async Task<ApiResultObject> GetResultsAsync()
        {
            var uri = new Uri($"{baseUrl}/{_key}/?type=koop&zo=/amsterdam/tuin/&page=1&pagesize=25");
            var results = await _httpClient.GetFromJsonAsync<ApiResultObject>(uri);
            return results;
        }
    }
}
