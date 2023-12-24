using FundaAggregator.Model;
using System.Text.Json;

namespace FundaAggregator.Tests;

public class ResultsAggregatorTests
{
    [Fact]
    public async Task TestResultsAggregator_GetTopMakelaars_ReturnsExpectedData()
    {
        using var fileStream = File.OpenRead(@"..\..\..\Assets\example-reponse-for-aggregation.json");
        
        var parsedResults = await JsonSerializer.DeserializeAsync<Listing[]>(fileStream);

        Assert.NotNull(parsedResults);

        var sut = new ResultsAggregator();

        // process the same batch multiple times to test aggregation
        sut.ProcessListingsBatch(parsedResults);
        sut.ProcessListingsBatch(parsedResults);
        sut.ProcessListingsBatch(parsedResults);

        var topMakelaars = sut.GetTopMakelaars(2);

        const int expectedTopMakelaarId = 24594;
        const int expectedSecondMakelaarId = 24585;

        Assert.Equal(2, topMakelaars.Count());

        Assert.Equal(expectedTopMakelaarId, topMakelaars.First().Key);
        Assert.Equal(6, topMakelaars.First().Value);

        Assert.Equal(expectedSecondMakelaarId, topMakelaars.ElementAt(1).Key);
        Assert.Equal(3, topMakelaars.ElementAt(1).Value);
    }
}
