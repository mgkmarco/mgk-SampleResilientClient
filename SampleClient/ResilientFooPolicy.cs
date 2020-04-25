using Polly;
using Polly.Extensions.Http;
using Polly.Fallback;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace SampleClient
{
    public class ResilientFooPolicy : DefaultPolicy
    {
        private readonly bool _businessAsUsual = true;

        public override IAsyncPolicy<HttpResponseMessage> GetPolicyWrap(HttpRequestMessage request)
        {
            if (request.Method != HttpMethod.Post && request.RequestUri.AbsolutePath.Contains("api/values"))
            {
                return GetFallbackPolicy().WrapAsync(base.GetPolicyWrap(request));
            }

            return Policy.NoOpAsync<HttpResponseMessage>();
        }

        protected AsyncFallbackPolicy<HttpResponseMessage> GetFallbackPolicy()
        {
            return Policy<HttpResponseMessage>
                .Handle<Exception>((ex) => _businessAsUsual)
                .OrTransientHttpError()
                .FallbackAsync(
                    fallbackAction: (context) =>
                    {
                        var defaultResponse = new DefaultFooResponse();
                        var response = new HttpResponseMessage();
                        response.Content =
                            new ObjectContent<DefaultFooResponse>(defaultResponse, new JsonMediaTypeFormatter());

                        return Task.FromResult(response);
                    });
        }
    }

    public class DefaultFooResponse
    {
        public string Name = "DefaultFooFallbackResponse";
    }
}