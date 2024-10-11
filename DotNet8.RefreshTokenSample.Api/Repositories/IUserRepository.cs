using DotNet8.RefreshTokenSample.Api.Models;
using DotNet8.RefreshTokenSample.Api.Utils;

namespace DotNet8.RefreshTokenSample.Api.Repositories
{
    public interface IUserRepository
    {

        Task<Result<JwtResponseModel>> LoginAsync(LoginRequestModel loginRequest, CancellationToken cs);
    }
}
