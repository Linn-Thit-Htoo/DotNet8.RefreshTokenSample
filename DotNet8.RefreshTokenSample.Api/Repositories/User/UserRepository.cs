using DotNet8.RefreshTokenSample.Api.AppDbContextModels;
using DotNet8.RefreshTokenSample.Api.Models;
using DotNet8.RefreshTokenSample.Api.Repositories.Jwt;
using DotNet8.RefreshTokenSample.Api.Services.SecurityServices;
using DotNet8.RefreshTokenSample.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.RefreshTokenSample.Api.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly AesService _aesService;
        private readonly IJWTManagerRepository _jwtManagerRepository;

        public UserRepository(AppDbContext context, AesService aesService, IJWTManagerRepository jwtManagerRepository)
        {
            _context = context;
            _aesService = aesService;
            _jwtManagerRepository = jwtManagerRepository;
        }

        public async Task<Result<Tokens>> DeleteRefreshTokenAsync(string refreshToken, int userId, CancellationToken cs = default)
        {
            Result<Tokens> result;
            try
            {
                var item = await _context.Tbl_Login.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken && x.UserId == userId, cancellationToken: cs);
                if (item is null)
                {
                    result = Result<Tokens>.NotFound();
                    goto result;
                }

                _context.Tbl_Login.Remove(item);
                await _context.SaveChangesAsync(cs);

                result = Result<Tokens>.Success();
            }
            catch (Exception ex)
            {
                result = Result<Tokens>.Fail(ex);
            }

        result:
            return result;
        }

        public async Task<Result<Tokens>> LoginAsync(LoginRequestModel loginRequest, CancellationToken cs)
        {
            Result<Tokens> result;
            try
            {
                var user = await _context.Tbl_User.FirstOrDefaultAsync(x => x.Email == loginRequest.Email && x.Password == loginRequest.Password, cancellationToken: cs);
                if (user is null)
                {
                    result = Result<Tokens>.NotFound();
                    goto result;
                }

                var jwt = GetJwtResponseModel(user);
                result = _jwtManagerRepository.GenerateTokens(jwt);

                await SaveRefreshTokenAsync(user.UserId, result.Data.RefreshToken, cs);
            }
            catch (Exception ex)
            {
                result = Result<Tokens>.Fail(ex);
            }

        result:
            return result;
        }

        public async Task SaveRefreshTokenAsync(int userId, string refreshToken, CancellationToken cs)
        {
            try
            {
                var user = await _context.Tbl_User.FindAsync([userId], cancellationToken: cs);
                ArgumentNullException.ThrowIfNull(user);

                var login = await _context.Tbl_Login.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken: cs);

                if (login is null)
                {
                    var model = new Tbl_Login
                    {
                        UserId = userId,
                        RefreshToken = refreshToken
                    };

                    await _context.Tbl_Login.AddAsync(model, cs);
                }
                else
                {
                    login.RefreshToken = refreshToken;
                    _context.Tbl_Login.Update(login);
                }

                await _context.SaveChangesAsync(cs);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private JwtResponseModel GetJwtResponseModel(Tbl_User tbl_User) => new(
            _aesService.Encrypt(tbl_User.UserId.ToString()),
            _aesService.Encrypt(tbl_User.UserName),
            _aesService.Encrypt(tbl_User.Email));
    }
}
