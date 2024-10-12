namespace DotNet8.RefreshTokenSample.Api.Repositories.User;

public interface IUserRepository
{
    Task SaveRefreshTokenAsync(int userId, string refreshToken, CancellationToken cs);
    Task<Result<Tokens>> DeleteRefreshTokenAsync(string refreshToken, int userId, CancellationToken cs = default);
    Task<Result<Tokens>> LoginAsync(LoginRequestModel loginRequest, CancellationToken cs);
}
