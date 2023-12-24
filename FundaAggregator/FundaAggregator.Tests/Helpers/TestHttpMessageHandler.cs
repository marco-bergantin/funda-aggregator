namespace FundaAggregator.Tests.Helpers;

internal class TestHttpMessageHandler(HttpResponseMessage[] httpResponseMessages) : HttpMessageHandler
{
    private readonly HttpResponseMessage[] _httpResponseMessages = httpResponseMessages;
    private int _requestCounter = 0;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_requestCounter >= _httpResponseMessages.Length)
        {
            throw new InvalidOperationException($"Cannot return HTTP response message {_requestCounter}");
        }

        return Task.FromResult(_httpResponseMessages[_requestCounter++]);
    }
}
