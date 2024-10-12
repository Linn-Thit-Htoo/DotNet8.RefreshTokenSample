namespace DotNet8.RefreshTokenSample.Api.Repositories.Jwt
{
    public class JWTManagerRepository : IJWTManagerRepository
    {
        private readonly IConfiguration _configuration;

        public JWTManagerRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Result<Tokens> GenerateTokens(JwtResponseModel jwtResponseModel)
        {
            Result<Tokens> result;
            try
            {
                var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("UserId", jwtResponseModel.UserId),
                new Claim("UserName", jwtResponseModel.UserName),
                new Claim("Email", jwtResponseModel.Email)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    _configuration["Jwt:Issuer"],
                    _configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddHours(10),
                    signingCredentials: signIn
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
                var refreshToken = GenerateRefreshToken();

                var model = new Tokens
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };

                result = Result<Tokens>.Success(model);
            }
            catch (Exception ex)
            {
                result = Result<Tokens>.Fail(ex);
            }

            return result;
        }

        public Result<ClaimsPrincipal> GetClaimsPrincipalFromExpireToken(string token)
        {
            Result<ClaimsPrincipal> result;
            try
            {
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
                var tokenValidParameter = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = _configuration.GetSection("Jwt:Issuer").Value,
                    ValidAudience = _configuration.GetSection("Jwt:Audience").Value
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidParameter, out SecurityToken securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token");
                }

                result = Result<ClaimsPrincipal>.Success(principal);
            }
            catch (Exception ex)
            {
                result = Result<ClaimsPrincipal>.Fail(ex);
            }

            return result;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
