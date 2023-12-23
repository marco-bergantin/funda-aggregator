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

var results = await apiClient.GetAllResultsAsync("koop", "/amsterdam/tuin/");

Console.WriteLine($"{results.Objects.Length} listings found{Environment.NewLine}");

var tableRows = string.Join(Environment.NewLine, 
    results.Objects.Select(o => 
        $"| {o.Id} | {o.Adres} | {o.KoopprijsTot} | {o.MakelaarId} | {o.MakelaarNaam} |"));

Console.WriteLine(tableRows);
Console.WriteLine(Environment.NewLine);

var aggregatedResults = ResultsAggregator.GetTopMakelaars(results, 10);

Console.WriteLine("TOP 10 Makelaars");

var top10MakelaarsTable = string.Join(Environment.NewLine,
    aggregatedResults.Keys.Select(makelaarId => $"| {makelaarId} | {aggregatedResults[makelaarId]} |"));

Console.WriteLine(top10MakelaarsTable);