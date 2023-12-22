namespace FundaAggregator.Tests.Helpers;

internal class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _httpResponseMessage;

    public TestHttpMessageHandler(HttpResponseMessage httpResponseMessage)
    {
        _httpResponseMessage = httpResponseMessage;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_httpResponseMessage);
    }
}
