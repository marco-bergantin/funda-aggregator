using Polly;
using Polly.Retry;

namespace FundaAggregator.Helpers;

public static class RetryStrategyOptionsProvider
{
    private const int MaxDelayAllowedInSeconds = 60;

    public static RetryStrategyOptions GetOptions(int maxSecondsDelay = MaxDelayAllowedInSeconds) =>
        new()
        {
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
            MaxRetryAttempts = 5,
            MaxDelay = maxSecondsDelay <= 0 || maxSecondsDelay > MaxDelayAllowedInSeconds
                ? throw new ArgumentException($"{nameof(maxSecondsDelay)} must be a positive int lower than {MaxDelayAllowedInSeconds}")
                : TimeSpan.FromSeconds(maxSecondsDelay)
        };
}
