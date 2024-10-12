using DotNet8.RefreshTokenSample.Api.Models;
using DotNet8.RefreshTokenSample.Api.Repositories.Jwt;
using DotNet8.RefreshTokenSample.Api.Repositories.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DotNet8.RefreshTokenSample.Api.Features.User
{
    [Route("api/account")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IJWTManagerRepository _jwtManagerRepository;
        private readonly IUserRepository _userRepository;

        public UserController(IJWTManagerRepository jwtManagerRepository, IUserRepository userRepository)
        {
            _jwtManagerRepository = jwtManagerRepository;
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginRequest, CancellationToken cs)
        {
            var result = await _userRepository.LoginAsync(loginRequest, cs);
            return Content(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestModel refreshTokenRequest, CancellationToken cs)
        {
            string authHeader = HttpContext.Request.Headers.Authorization!;
            var header_token = authHeader.Split(" ");
            var oldToken = header_token[1];

            var principalResult = _jwtManagerRepository.GetClaimsPrincipalFromExpireToken(oldToken);
            if (!principalResult.IsSuccess)
            {
                return Content(principalResult);
            }

            var principal = principalResult.Data;
            string encryptedUserId = principal.FindFirstValue("UserId")!;
            string encryptedUserName = principal.FindFirstValue("UserName")!;
            string encryptedEmail = principal.FindFirstValue("Email")!;

            // generate new tokens
            var jwt = new JwtResponseModel(encryptedUserId, encryptedUserName, encryptedEmail);
            var tokensResult = _jwtManagerRepository.GenerateTokens(jwt);

            // delete refresh token
            var deleteRefreshTokenResult = await _userRepository.DeleteRefreshTokenAsync(refreshTokenRequest.RefreshToken, refreshTokenRequest.UserId, cs);
            if (!deleteRefreshTokenResult.IsSuccess)
            {
                return Content(deleteRefreshTokenResult);
            }

            // save new refresh token
            await _userRepository.SaveRefreshTokenAsync(refreshTokenRequest.UserId, tokensResult.Data.RefreshToken, cs);

            return Content(tokensResult);
        }
    }
}
