# funda-aggregator

## Assignment

The funda API returns information about the objects that are listed on funda.nl which are for sale.

An example of one of the URLs in our REST API is:  
http://partnerapi.funda.nl/feeds/Aanbod.svc/[key]/?type=koop&zo=/amsterdam/tuin/&page=1&pagesize=25

Most of the parameters that you can pass are self explanatory. The parameter 'zo' (zoekopdracht or
search query) is the same as the key used in funda.nl URLs. For example the URL shown above
searches for houses in Amsterdam with a garden: http://www.funda.nl/koop/amsterdam/tuin/.
The API returns data about the object in XML. If you add the key 'json' between 'Aanbod.svc' and
[key], a JSON representation of the data will be returned instead of XML.


The assignment
Determine which makelaar's in Amsterdam have the most object listed for sale. Make a table of the
top 10. Then do the same thing but only for objects with a tuin which are listed for sale.  
For the
assignment you may write a program in any object oriented language of your choice and you may
use any libraries that you find useful.

## How to run

```
git clone https://github.com/marco-bergantin/funda-aggregator.git
cd .\funda-aggregator\FundaAggregator\FundaAggregator\
dotnet user-secrets set 'FundaPartnerApiKey' '<your-key-here>'
dotnet run "koop" "/amsterdam/"
dotnet run "koop" "/amsterdam/tuin/"
```

## Considerations

- The solution is a console app, which accepts two parameters as arguments: the type of search, e.g. "koop" and the search query, e.g. "/amsterdam/tuin/"
- It expects the base URL of funda's partner API in the `appSettings.json` (already provided) and the API key as a [user secret](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows)
  - the latter safely stores the key locally and helps with not accidentally checking it into a repo
- The API seems to be based on a WCF service: I spent some time researching how I could easily generate a client for it, and tried out [dotnet's new wcf libraries](https://github.com/dotnet/wcf), but 
  - it seems a bit overkill to generate the client for all endpoints and operations (I only needed one)
  - it resulted in a lot of generated code
  - support for .NET 8 is not shipped yet (see https://github.com/dotnet/wcf/issues/5362)
  - so I opted for writing a simple HTTP client based solution, which allowed me to use some of the latest C# features
- The core of the program's logic is split in two classes, [FundaPartnerApiClient](FundaAggregator/FundaAggregator/FundaPartnerApiClient.cs) and [ResultsAggregator](FundaAggregator/FundaAggregator/ResultsAggregator.cs)
- `FundaPartnerApiClient` handles the communication with the API
  - its constructor accepts 4 parameters:
    - an `HttpClient`, which allows the consumer to determine how this will be created (e.g. one could use [IHttpClientFactory](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-8.0) in an ASP.NET Core application) and its lifecycle
    - the `baseApiUri` for flexibility (think of testing against multiple environments, test/prod) and good practices of not hardcoding URIs. Here as well, it again allow the consumer to handle settings and configuration as it best fits the type of application
    - similarly for the `apiKey`, think of storing it locally with user secrets or in a [Key Vault](https://azure.microsoft.com/en-us/products/key-vault), for example
    - `retryOptions` to control the resiliency logic, implemented using [Polly](https://github.com/App-vNext/Polly): other than the considerations that apply to the reasoning behind the other parameters, this comes handy for testing the retry on failing HTTP calls, so that in the tests one can set the maxDelay to 1 second (rather than 60) and avoid having tests that run for too long.
  - the `GetResultsAsync` method handles the fecthing of a single page of results from funda's partner API, including the use of the retry policy. This is a simple retry with exponential backoff and jitter that retries on any type of HTTP response failure, see [RetryStrategyOptionsProvider](FundaAggregator/FundaAggregator/Helpers/RetryStrategyOptionsProvider.cs)
    - here, I chose a policy that was both simple and safe enough to ensure the app gets all the data
    - for a production application, I would gather metrics from actual usage (failure rates, breakdown per status code, response time) to come up with a policy that fits the real use case.
  - the `GetAllResultsAsync` method gets all the data from the API for the specified query, by "navigating" all pages.
    - this is implemented leveraging the [IAsyncEnumerable feature](https://learn.microsoft.com/en-us/archive/msdn-magazine/2019/november/csharp-iterating-with-async-enumerables-in-csharp-8), which allows the client to consume the data as it becomes available, as opposed to having to wait for all the data to be available to start consuming it
    - this, in turn, means that, while `FundaPartnerApiClient` is waiting for the HTTP response from the API to get e.g. page 2 out of 25 for the results, the consumer code in Program.cs can already start processing the data from the first page to (in this case) display it on the console and to start aggregating it
    - this awesome feature comes with some of the same caveats of the regular `IEnumerable<T>` collections, namely deferred execution/lazy evaluation (i.e. the collection is actually materialized only when iterating on it, unless you call e.g. `.ToArray()` on it) and cost of multiple enumeration (i.e. if you iterate multiple times over the collection, then the expression generated will be evaluated as many times), which means that, for example, if you were to pass around the result of `apiClient.GetAllResultsAsync` and iterate multiple times over it, you'd be repeating all the HTTP calls to the API, with the retry logic included! 
- `ResultsAggregator` will keep track of how many listings each makelaar has on funda, by processing each batch as it comes in (see previous point)
  - this means that, as soon as all pages are fetched, the data is already aggregated and only need to be sorted and displayed
  - I preferred to aggregate by MakelaarId (a number) rather than MakelaarName (a string), because it seemed safer to assume that those Ids are unique, rather than the name. It would be easy to change this to produce the top 10 makelaars table by using names, but this seemed reasonable enough.
  - NB: the `Dictionary<int, int>` used internally by this class will grow to a number of entries that's equal to the total number of makelaars who have published listings that are returned by the user specified query.
    - this could be improved to only keep the data of the top N (10, in our case) makelaars, but since in this context we're talking about only a couple hundreds entries, I preferred to keep the code as simple as possible and stick with this solution.
- I wrote a few tests to ensure no functionality would break as I was iterating on my solution. I know this wasn't mandatory, but another benefit of doing this is that you're forced to write testable code (which was a requirement and generally a good idea)
  - in the spirit of keeping it simple, I refrained from using interfaces, dependency injection or any mocking library, which made for less code and more realistic tests, I believe.
