using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SampleClient
{
    public class ResilientDelegationHandler : DelegatingHandler
    {
        private readonly IPolicy _policy;

        public ResilientDelegationHandler(IPolicy policy)
        {
            _policy = policy;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return _policy.GetPolicyWrap(request).ExecuteAsync(ct => base.SendAsync(request, ct), cancellationToken);
        }
    }
}