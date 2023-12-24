using FundaAggregator.Model;

namespace FundaAggregator;

public class ResultsAggregator
{
    private readonly Dictionary<int, int> _listingsPerMakelaar = [];

    public void ProcessListingsBatch(IEnumerable<Listing> listingsBatch)
    {
        foreach (var listing in listingsBatch)
        {
            if (_listingsPerMakelaar.TryGetValue(listing.MakelaarId, out int value))
            {
                _listingsPerMakelaar[listing.MakelaarId] = ++value;
            }
            else
            {
                _listingsPerMakelaar.Add(listing.MakelaarId, 1);
            }
        }
    }

    public IEnumerable<KeyValuePair<int, int>> GetTopMakelaars(uint top)
    {
        return _listingsPerMakelaar.OrderByDescending(pair => pair.Value).Take((int)top);
    }
}
