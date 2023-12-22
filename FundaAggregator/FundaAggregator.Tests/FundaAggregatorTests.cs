using FundaAggregator.Tests.Helpers;
using System.Net;

namespace FundaAggregator.Tests
{
    public class FundaAggregatorTests
    {
        [Fact]
        public async void TestApiClient_GetResultsAsync_DeserializesDataCorrectly()
        {
            var fileContents = File.ReadAllText(@"..\..\..\Assets\example-reponse.json");

            var testHttpMessageHandler = new TestHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(fileContents)
            });

            var sut = new FundaPartnerApiClient(new HttpClient(testHttpMessageHandler),
                "key-doesnt-matter-here");

            var results = await sut.GetResultsAsync();

            Assert.NotNull(results);
            Assert.NotNull(results.Objects);

            Assert.Equal(3, results.Objects.Length);
            Assert.True(results.Objects.All(o => !string.IsNullOrWhiteSpace(o.Id) && o.Koopprijs > 0));
        }
    }
}