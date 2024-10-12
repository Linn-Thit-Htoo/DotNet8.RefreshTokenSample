using DotNet8.RefreshTokenSample.Api.Models;
using DotNet8.RefreshTokenSample.Api.Repositories.Jwt;
using DotNet8.RefreshTokenSample.Api.Repositories.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
