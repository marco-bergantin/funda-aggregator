using FundaAggregator.Model;

namespace FundaAggregator;

public class ResultsAggregator
{
    public static async Task<IEnumerable<KeyValuePair<int, int>>> GetTopMakelaars(IAsyncEnumerable<Listing[]> results, int top)
    {
        var listingsPerMakelaar = new Dictionary<int, int>();

        await foreach (var listingsBatch in results)
        {
            foreach (var listing in listingsBatch)
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
        }

        return listingsPerMakelaar.OrderByDescending(pair => pair.Value).Take(top);
    }
}
