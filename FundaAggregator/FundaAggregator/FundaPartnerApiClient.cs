using FundaAggregator.Model;
using System.Net.Http.Json;

namespace FundaAggregator
{
    public class FundaPartnerApiClient
    {
        private static string baseUrl = "http://partnerapi.funda.nl/feeds/Aanbod.svc/json";

        private HttpClient _httpClient;
        private string _key;

        public FundaPartnerApiClient(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _key = apiKey;
        }

        public async Task<ApiResultObject> GetResultsAsync(int page = 1, int pageSize = 25)
        {
            var uri = new Uri($"{baseUrl}/{_key}/?type=koop&zo=/amsterdam/tuin/&page={page}&pagesize={pageSize}");
            var results = await _httpClient.GetFromJsonAsync<ApiResultObject>(uri);
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

                await Task.Delay(100); // TODO: implement proper solution for not getting throttled
            }

            return new ApiResultObject { Objects = [.. allResults] };
        }
    }
}
