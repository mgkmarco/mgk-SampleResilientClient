using Polly;
using Polly.Extensions.Http;
using Polly.Fallback;
using Polly.Retry;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace SampleClient
{
    public class ResilientBarPolicy : DefaultPolicy
    {
        private readonly bool _businessAsUsual = true;
        private const int _retryCount = 5;
        private readonly TimeSpan _sleepDuration = TimeSpan.FromSeconds(10);

        public override IAsyncPolicy<HttpResponseMessage> GetPolicyWrap(HttpRequestMessage request)
        {
            if (request.Method != HttpMethod.Post && request.RequestUri.AbsolutePath.Contains("api/values"))
            {
                return GetFallbackPolicy().WrapAsync(base.GetPolicyWrap(request));
            }

            return Policy.NoOpAsync<HttpResponseMessage>();
        }

        protected override AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return Policy
                .Handle<Exception>()
                .OrTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: _retryCount,
                    sleepDurationProvider: (retry) => _sleepDuration,
                    onRetry: (exception, retryCount, context) => Console.WriteLine($"retried after sleep: {retryCount}")
                );
        }

        protected AsyncFallbackPolicy<HttpResponseMessage> GetFallbackPolicy()
        {
            return Policy<HttpResponseMessage>
                .Handle<Exception>((ex) => _businessAsUsual)
                .OrTransientHttpError()
                .FallbackAsync(
                    fallbackAction: (context) =>
                    {
                        var defaultResponse = new DefaultBarResponse();
                        var response = new HttpResponseMessage();
                        response.Content =
                            new ObjectContent<DefaultBarResponse>(defaultResponse, new JsonMediaTypeFormatter());

                        return Task.FromResult(response);
                    });
        }
    }

    public class DefaultBarResponse
    {
        public string Name = "DefaultBarFallbackResponse";
        public string AnotherName = "AnotherName";
    }
}