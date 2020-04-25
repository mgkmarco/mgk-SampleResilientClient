using System;
using System.Net.Http;

namespace SampleClient
{
    public class ClientFactory
    {
        public static HttpClient FooHttpClient { get; } =
            GetFooHttpClient();

        public static HttpClient BarHttpClient { get; } =
            GetBarHttpClient();

        private static HttpClient GetFooHttpClient()
        {
            var resilientFooPolicy = new ResilientFooPolicy();
            var resilientDelegatingHandler = new ResilientDelegationHandler(resilientFooPolicy);
            var httpClient = HttpClientFactory.Create(resilientDelegatingHandler);
            httpClient.BaseAddress = new Uri("https://localhost:44337");

            return httpClient;
        }

        private static HttpClient GetBarHttpClient()
        {
            var resilientBarPolicy = new ResilientBarPolicy();
            var resilientDelegatingHandler = new ResilientDelegationHandler(resilientBarPolicy);
            var httpClient = HttpClientFactory.Create(resilientDelegatingHandler);
            httpClient.BaseAddress = new Uri("https://localhost:44320");

            return httpClient;
        }
    }
}