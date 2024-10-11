using DotNet8.RefreshTokenSample.Api.Models;
using DotNet8.RefreshTokenSample.Api.Utils;

namespace DotNet8.RefreshTokenSample.Api.Repositories.User
{
    public interface IUserRepository
    {

        Task<Result<Tokens>> LoginAsync(LoginRequestModel loginRequest, CancellationToken cs);
    }
}
