using System.Net.Http.Headers;

namespace ClientesAPI.Data
{
    public class PropagateJwtHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public PropagateJwtHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _contextAccessor?
                .HttpContext.Request.Headers["Authorization"]
                .ToString()
                .Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
