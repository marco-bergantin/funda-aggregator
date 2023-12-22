// See https://aka.ms/new-console-template for more information
using FundaAggregator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// dotnet user-secrets set "FundaPartnerApiKey" "<your-key>"
var apiKey = config["FundaPartnerApiKey"];

var services = new ServiceCollection();
services.AddHttpClient();
var serviceProvider = services.BuildServiceProvider();

var httpClient = serviceProvider.GetService<HttpClient>();

var apiClient = new FundaPartnerApiClient(httpClient, apiKey);

var results = await apiClient.GetResultsAsync();

var tableRows = string.Join(Environment.NewLine, 
    results.Objects.Select(o => 
        $"| {o.Id} | {o.Adres} | {o.KoopprijsTot} | {o.MakelaarId} | {o.MakelaarNaam} |"));

Console.WriteLine(tableRows);
