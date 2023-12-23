using FundaAggregator.Model;
using System.Text.Json;

namespace FundaAggregator.Tests;

public class ResultsAggregatorTests
{
    [Fact]
    public async Task TestResultsAggregator_GetTopMakelaars_ReturnsExpectedData()
    {
        using var fileStream = File.OpenRead(@"..\..\..\Assets\example-reponse-for-aggregation.json");
        
        var parsedResults = JsonSerializer.DeserializeAsyncEnumerable<Listing[]>(fileStream);

        Assert.NotNull(parsedResults);

        var topMakelaars = await ResultsAggregator.GetTopMakelaars(parsedResults, 2);

        Assert.Equal(2, topMakelaars.Count());

        Assert.Equal(24594, topMakelaars.First().Key);
        Assert.Equal(2, topMakelaars.First().Value);

        Assert.Equal(24585, topMakelaars.ElementAt(1).Key);
        Assert.Equal(1, topMakelaars.ElementAt(1).Value);
    }
}
