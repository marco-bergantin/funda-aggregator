using FundaAggregator.Model;
using System.Text.Json;

namespace FundaAggregator.Tests;

public class ResultsAggregatorTests
{
    [Fact]
    public async Task TestResultsAggregator_GetTopMakelaars_ReturnsExpectedData()
    {
        using var fileStream = File.OpenRead(@"..\..\..\Assets\example-reponse-for-aggregation.json");
        
        var parsedApiResult = await JsonSerializer.DeserializeAsync<ApiResultObject>(fileStream);

        Assert.NotNull(parsedApiResult);

        var topMakelaars = ResultsAggregator.GetTopMakelaars(parsedApiResult, 2);

        Assert.Equal(2, topMakelaars.Count);

        Assert.Equal(24594, topMakelaars.Keys.First());
        Assert.Equal(2, topMakelaars[24594]);

        Assert.Equal(24585, topMakelaars.Keys.ElementAt(1));
        Assert.Equal(1, topMakelaars[24585]);
    }
}
