using FundaAggregator.Model;

namespace FundaAggregator;

public class ResultsAggregator
{
    public static IDictionary<int, int> GetTopMakelaars(ApiResultObject results, int top)
    {
        var listingsPerMakelaar = new Dictionary<int, int>();

        foreach (var listing in results.Objects)
        {
            if (listingsPerMakelaar.TryGetValue(listing.MakelaarId, out int value))
            {
                listingsPerMakelaar[listing.MakelaarId] = ++value;
            }
            else
            {
                listingsPerMakelaar.Add(listing.MakelaarId, 1);
            }
        }

        // TODO: there's probably a better way to do this
        var top10ByListings = listingsPerMakelaar.OrderByDescending(pair => pair.Value)
            .Take(top)
            .ToDictionary();
        
        return top10ByListings;
    }
}
