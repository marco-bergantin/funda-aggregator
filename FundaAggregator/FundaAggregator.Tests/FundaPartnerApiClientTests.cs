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
            new Uri("https://localhost:8080/doesn-matter-for-these-tests"),
            "key-doesnt-matter-here");

        var results = await sut.GetResultsAsync("test", "/whatever/irrelevant/");

        Assert.NotNull(results);
        Assert.NotNull(results.Listings);

        Assert.Equal(3, results.Listings.Length);
        Assert.True(results.Listings.All(o => !string.IsNullOrWhiteSpace(o.Id) && o.Koopprijs > 0));
    }

    [Fact]
    public async Task TestApiClient_GetAllResultsAsync_NavigatesPagingCorrectly()
    {
        var firstPageContent = new ApiResultObject
        {
            Listings = 
            [
                new Listing 
                {
                    Id = "0"
                },
                new Listing
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
            Listings =
            [
                new Listing
                {
                    Id = "2"
                },
                new Listing
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
            new Uri("https://localhost:8080/doesn-matter-for-these-tests"),
            "key-doesnt-matter-here");

        var results = sut.GetAllResultsAsync("test", "/whatever/irrelevant/");

        Assert.NotNull(results);

        await AssertOnResultsAsync(results, 4);
    }

    [Fact]
    public async Task TestApiClient_GetAllResultsAsync_HandlesTransientErrors()
    {
        var firstPageContent = new ApiResultObject
        {
            Listings =
            [
                new Listing
                {
                    Id = "0"
                }
            ],
            Paging = new Paging
            {
                HuidigePagina = 1,
                AantalPaginas = 3
            }
        };

        var secondPageContent = new ApiResultObject
        {
            Listings =
            [
                new Listing
                {
                    Id = "1"
                }
            ],
            Paging = new Paging
            {
                HuidigePagina = 2,
                AantalPaginas = 3
            }
        };

        var thirdPageContent = new ApiResultObject
        {
            Listings =
            [
                new Listing
                {
                    Id = "2"
                }
            ],
            Paging = new Paging
            {
                HuidigePagina = 3,
                AantalPaginas = 3
            }
        };

        var testHttpMessageHandler = new TestHttpMessageHandler([
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(firstPageContent))
            },
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Server side error!")
            },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(secondPageContent))
            },
            new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("Come back later")
            },
            new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("Whoops")
            },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(thirdPageContent))
            }
        ]);

        var sut = new FundaPartnerApiClient(new HttpClient(testHttpMessageHandler),
            new Uri("https://localhost:8080/doesn-matter-for-these-tests"),
            "key-doesnt-matter-here");

        var results = sut.GetAllResultsAsync("test", "/whatever/irrelevant/");

        Assert.NotNull(results);

        await AssertOnResultsAsync(results, 3);
    }

    [Fact]
    public async Task TestApiClient_GetAllResultsAsync_FailsAfter4ConsecutiveErrors()
    {
        var testHttpMessageHandler = new TestHttpMessageHandler([
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("Server side error!")
            },
            new HttpResponseMessage(HttpStatusCode.BadGateway)
            {
                Content = new StringContent("Whoops")
            },
            new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("Come back later")
            },
            new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                Content = new StringContent("Whacky throttling")
            },
            new HttpResponseMessage(HttpStatusCode.TooManyRequests)
            {
                Content = new StringContent("Dont come back!")
            },
            new HttpResponseMessage(HttpStatusCode.Forbidden)
            {
                Content = new StringContent("Whackier throttling")
            }
        ]);

        var sut = new FundaPartnerApiClient(new HttpClient(testHttpMessageHandler),
            new Uri("https://localhost:8080/doesn-matter-for-these-tests"),
            "key-doesnt-matter-here");

        var results = sut.GetAllResultsAsync("test", "/whatever/irrelevant/");

        await Assert.ThrowsAsync<HttpRequestException>(async () => 
        {
            // NB exception won't be thrown if you don't materialize the results
            await foreach (var listingsBatch in results) { } 
        }); 
    }

    private static async Task AssertOnResultsAsync(IAsyncEnumerable<Listing[]> results,
        int expectedTotal)
    {
        var totalCounter = 0;
        await foreach (var listingsBatch in results)
        {
            for (int i = 0; i < listingsBatch.Length; i++, totalCounter++)
            {
                Assert.Equal(totalCounter.ToString(), listingsBatch[i].Id);
            }
        }

        Assert.Equal(expectedTotal, totalCounter);
    }
}