using Polly;
using Polly.Retry;
using System;
using System.Net;
using System.Net.Http;

namespace Scraper.API.Helpers
{
    public class PolicyProvider
    {
        public static RetryPolicy<HttpResponseMessage> WaitAndRetry()
        {
            var waitAndRetryPolicy = Policy
                .HandleResult<HttpResponseMessage>(e => e.StatusCode == HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(10, // Retry 10 times with a delay between retries before ultimately giving up
                    attempt => TimeSpan.FromSeconds(0.25 * Math.Pow(2, attempt)) // Back off!  2, 4, 8, 16 etc times 1/4-second
                );

            return waitAndRetryPolicy;
        }
    }
}