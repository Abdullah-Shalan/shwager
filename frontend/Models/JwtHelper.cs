using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace frontend.Models;
public static class JwtHelper
{
    public static IEnumerable<Claim> GetClaims(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Enumerable.Empty<Claim>();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        return jwtToken.Claims;
    }

    public static string? GetClaimValue(string token, string claimType)
    {
        var claim = GetClaims(token).FirstOrDefault(c => c.Type == claimType);
        return claim?.Value;
    }
}
