using DotNet8.RefreshTokenSample.Api.Models;
using DotNet8.RefreshTokenSample.Api.Utils;
using System.Security.Claims;

namespace DotNet8.RefreshTokenSample.Api.Repositories.Jwt
{
    public interface IJWTManagerRepository
    {
        Result<Tokens> GenerateTokens(JwtResponseModel jwtResponseModel);
        string GenerateRefreshToken();
        ClaimsPrincipal GetClaimsPrincipalFromExpireToken(string token);
    }
}
