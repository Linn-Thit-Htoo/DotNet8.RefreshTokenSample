using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNet8.RefreshTokenSample.Api.Services.AuthServices
{
    public class TokenValidationService
    {
        private readonly IConfiguration _configuration;

        public TokenValidationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new();
                byte[] key = Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt:Key").Value!);

                TokenValidationParameters parameters =
                    new()
                    {
                        RequireExpirationTime = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidAudience = _configuration["Jwt:Audience"],
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                ClaimsPrincipal principal = tokenHandler.ValidateToken(
                    token,
                    parameters,
                    out SecurityToken securityToken
                );

                return principal;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}
