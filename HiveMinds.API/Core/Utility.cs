using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HiveMinds.API.Core;

public abstract class Utility
{
    public static int? GetAccountIdFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token);
        var tokenS = jsonToken as JwtSecurityToken;
        var accountId = tokenS?.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        return accountId == null ? null : int.Parse(accountId);
    }

    public static int GetAccountIdFromClaims(ClaimsPrincipal user)
    {
        var accountId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (accountId != null && int.TryParse(accountId, out var id))
            return id;
        return 0;
    }
}