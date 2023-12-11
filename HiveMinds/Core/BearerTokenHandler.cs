using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;

namespace HiveMinds.Core;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_httpContextAccessor.HttpContext == null) return await base.SendAsync(request, cancellationToken);

        var token = _httpContextAccessor.HttpContext.Request.Cookies["token"];
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            await _httpContextAccessor.HttpContext.SignOutAsync();
        }

        return await base.SendAsync(request, cancellationToken);
    }
}