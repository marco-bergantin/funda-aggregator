using FundaAggregator.Model;
using FundaAggregator.Tests.Helpers;
using System.Net;
using System.Text.Json;

namespace FundaAggregator.Tests;

public class FundaPartnerApiClientTests
{
    [Fact]
    public async Task TestApiClient_GetResultsAsync_DeserializesDataCorrectly()
    {
        var fileContents = File.ReadAllText(@"..\..\..\Assets\example-reponse.json");

        var testHttpMessageHandler = new TestHttpMessageHandler([new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(fileContents)
        }]);

        var sut = new FundaPartnerApiClient(new HttpClient(testHttpMessageHandler),
            "key-doesnt-matter-here");

        var results = await sut.GetResultsAsync();

        Assert.NotNull(results);
        Assert.NotNull(results.Objects);

        Assert.Equal(3, results.Objects.Length);
        Assert.True(results.Objects.All(o => !string.IsNullOrWhiteSpace(o.Id) && o.Koopprijs > 0));
    }

    [Fact]
    public async Task TestApiClient_GetAllResultsAsync_NavigatesPagingCorrectly()
    {
        var firstPageContent = new ApiResultObject
        {
            Objects = 
            [
                new Model.Object 
                {
                    Id = "0"
                },
                new Model.Object
                {
                    Id = "1"
                }
            ],
            Paging = new Paging 
            {
                HuidigePagina = 1,
                AantalPaginas = 2
            }
        };

        var secondPageContent = new ApiResultObject
        {
            Objects =
            [
                new Model.Object
                {
                    Id = "2"
                },
                new Model.Object
                {
                    Id = "3"
                }
            ],
            Paging = new Paging
            {
                HuidigePagina = 2,
                AantalPaginas = 2
            }
        };

        var testHttpMessageHandler = new TestHttpMessageHandler([
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(firstPageContent))
            },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(secondPageContent))
            }
        ]);

        var sut = new FundaPartnerApiClient(new HttpClient(testHttpMessageHandler),
            "key-doesnt-matter-here");

        var results = await sut.GetAllResultsAsync();

        Assert.NotNull(results);
        Assert.NotNull(results.Objects);

        Assert.Equal(4, results.Objects.Length);

        for (int i = 0; i < results.Objects.Length; i++)
        {
            Assert.Equal(i.ToString(), results.Objects[i].Id);
        }
    }
}