namespace FundaAggregator.Tests.Helpers;

internal class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage[] _httpResponseMessages;
    private int _requestCounter = 0;

    public TestHttpMessageHandler(HttpResponseMessage[] httpResponseMessages)
    {
        _httpResponseMessages = httpResponseMessages;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_requestCounter >= _httpResponseMessages.Length)
        {
            throw new InvalidOperationException($"Cannot return HTTP response message {_requestCounter}");
        }

        return Task.FromResult(_httpResponseMessages[_requestCounter++]);
    }
}
