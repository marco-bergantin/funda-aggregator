// See https://aka.ms/new-console-template for more information
using FundaAggregator;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .AddJsonFile("appSettings.json")
    .Build();

var apiKey = config[SettingKeys.ApiSecret];
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new ArgumentException($"{SettingKeys.ApiSecret} not set in user secrets. " +
        $"Please run 'dotnet user-secrets set \"FundaPartnerApiKey\" \"<your-key>\"'");
}

var baseApiUrl = config[SettingKeys.ApiBaseUrl];
if (string.IsNullOrWhiteSpace(baseApiUrl) 
    || !Uri.TryCreate(baseApiUrl, UriKind.Absolute, out var baseApiUri))
{
    throw new ArgumentException($"{SettingKeys.ApiBaseUrl} not a valid uri. " +
        $"Please provide a valid absolute uri in appSettings.json");
}

using var httpClient = new HttpClient();

var apiClient = new FundaPartnerApiClient(httpClient, baseApiUri, apiKey);

Console.WriteLine("Fetching results from funda partner api...");
Console.WriteLine(Environment.NewLine);

var results = apiClient.GetAllResultsAsync("koop", "/amsterdam/tuin/");
await foreach (var listingsBatch in results)
{
    var tableRows = string.Join(Environment.NewLine, listingsBatch.Select(o =>
        $"| {o.Id} | {o.Adres} | {o.KoopprijsTot} | {o.MakelaarId} | {o.MakelaarNaam} |"));

    Console.WriteLine(tableRows);
}

Console.WriteLine(Environment.NewLine);
Console.WriteLine("Processing results...");

var aggregatedResults = await ResultsAggregator.GetTopMakelaars(results, 10);

Console.WriteLine("TOP 10 Makelaars");

var top10MakelaarsTable = string.Join(Environment.NewLine,
    aggregatedResults.Select(makelaarData => $"| {makelaarData.Key} | {makelaarData.Value} |"));

Console.WriteLine(top10MakelaarsTable);