using Polly;
using System.Net.Http;

namespace SampleClient
{
    public interface IPolicy
    {
        IAsyncPolicy<HttpResponseMessage> GetPolicyWrap(HttpRequestMessage request);
    }
}