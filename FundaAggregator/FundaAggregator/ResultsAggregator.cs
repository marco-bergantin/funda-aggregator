using FundaAggregator.Model;

namespace FundaAggregator;

public class ResultsAggregator
{
    public static IEnumerable<KeyValuePair<int, int>> GetTopMakelaars(IEnumerable<Listing> listings, int top)
    {
        var listingsPerMakelaar = new Dictionary<int, int>();

        foreach (var listing in listings)
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

        return listingsPerMakelaar.OrderByDescending(pair => pair.Value).Take(top);
    }
}
