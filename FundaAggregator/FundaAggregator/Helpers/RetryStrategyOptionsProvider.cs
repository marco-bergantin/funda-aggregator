using Polly;
using Polly.Retry;

namespace FundaAggregator.Helpers;

public static class RetryStrategyOptionsProvider
{
    private const int MaxDelayAllowedInSconds = 60;

    public static RetryStrategyOptions GetOptions(int maxSecondsDelay = MaxDelayAllowedInSconds) =>
        new()
        {
            BackoffType = DelayBackoffType.Exponential,
            UseJitter = true,
            ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
            MaxRetryAttempts = 5,
            MaxDelay = maxSecondsDelay < 0 || maxSecondsDelay > MaxDelayAllowedInSconds
                ? throw new ArgumentException($"{nameof(maxSecondsDelay)} must be a positive int lower than {MaxDelayAllowedInSconds}")
                : TimeSpan.FromSeconds(maxSecondsDelay)
        };
}
