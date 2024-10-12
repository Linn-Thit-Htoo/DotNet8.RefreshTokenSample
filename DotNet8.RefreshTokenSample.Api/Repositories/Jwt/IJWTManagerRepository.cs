namespace DotNet8.RefreshTokenSample.Api.Repositories.Jwt;

public interface IJWTManagerRepository
{
    Result<Tokens> GenerateTokens(JwtResponseModel jwtResponseModel);
    string GenerateRefreshToken();
    Result<ClaimsPrincipal> GetClaimsPrincipalFromExpireToken(string token);
}
