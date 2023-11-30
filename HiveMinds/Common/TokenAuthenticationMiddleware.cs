using Microsoft.AspNetCore.Authentication;

namespace HiveMinds.Common;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenAuthenticationMiddleware> _logger;

    private readonly List<string> _allowedPaths = new()
    {
        "/Login",
        "/Signup"
    };

    public TokenAuthenticationMiddleware(RequestDelegate next, ILogger<TokenAuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Cookies["token"];
        var path = context.Request.Path;


        if (string.IsNullOrEmpty(token) && !_allowedPaths.Any(p => path.StartsWithSegments(p)))
        {
            await context.SignOutAsync();
            context.Response.Redirect("/Login");
        }
        else
        {
            await _next(context);
        }
    }
}