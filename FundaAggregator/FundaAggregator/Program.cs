// See https://aka.ms/new-console-template for more information
using FundaAggregator;
using FundaAggregator.Helpers;
using FundaAggregator.Model;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appSettings.json")
    .Build();

if (args.Length < 2)
{
    Console.WriteLine("Please provide two parameters: " +
        "one for type of search (e.g. 'koop') and one for search query " +
        "(zoekopdracht, e.g. '/amsterdam/tuin/')");
    return 1;
}

var typeSearch = args[0];
if (string.IsNullOrWhiteSpace(typeSearch))
{
    Console.WriteLine("Please provide a valid search type, e.g. 'koop'");
    return 1;
}

var searchQuery = args[1];
if (string.IsNullOrWhiteSpace(searchQuery))
{
    Console.WriteLine("Please provide a valid search query, e.g. '/amsterdam/tuin/'");
    return 1;
}

var apiKey = config[SettingKeys.ApiSecret];
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine($"{SettingKeys.ApiSecret} not set in user secrets.");
    Console.WriteLine($"Please run 'dotnet user-secrets set \"FundaPartnerApiKey\" \"<your-key>\"'");
    return 1;
}

var baseApiUrl = config[SettingKeys.ApiBaseUrl];
if (string.IsNullOrWhiteSpace(baseApiUrl) 
    || !Uri.TryCreate(baseApiUrl, UriKind.Absolute, out var baseApiUri))
{
    Console.WriteLine($"{SettingKeys.ApiBaseUrl} not a valid uri.");
    Console.WriteLine("Please provide a valid absolute uri in appSettings.json");
    return 1;
}

try
{
    using var httpClient = new HttpClient();

    var apiClient = new FundaPartnerApiClient(httpClient, 
        baseApiUri, apiKey, RetryStrategyOptionsProvider.GetOptions());

    Console.WriteLine($"Fetching results from funda partner api for type {typeSearch}, search query {searchQuery}");
    Console.WriteLine(Environment.NewLine);
    Console.WriteLine("| ListingId | Postcode | Price | MakelaarId | MakelaarName |");

    var aggregator = new ResultsAggregator();
    var results = apiClient.GetAllResultsAsync(typeSearch, searchQuery);

    await foreach (var listingsBatch in results)
    {
        aggregator.ProcessListingsBatch(listingsBatch);

        var tableRows = string.Join(Environment.NewLine, listingsBatch.Select(o =>
            $"| {o.GlobalId} | {o.Postcode} | {o.Koopprijs} | {o.MakelaarId} | {o.MakelaarNaam} |"));

        Console.WriteLine(tableRows);
    }

    var aggregatedResults = aggregator.GetTopMakelaars(10);

    Console.WriteLine(Environment.NewLine);
    Console.WriteLine("TOP 10 Makelaars");
    Console.WriteLine("| Id | Total Listings |");

    var top10MakelaarsTable = string.Join(Environment.NewLine,
        aggregatedResults.Select(makelaarData => $"| {makelaarData.Key} | {makelaarData.Value} |"));

    Console.WriteLine(top10MakelaarsTable);
}
catch (Exception ex)
{
    Console.WriteLine($"Error occurred, {ex.GetType().Name}: {ex.Message}");
    return 1;
}

return 0;