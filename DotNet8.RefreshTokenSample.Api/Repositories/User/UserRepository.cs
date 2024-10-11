using DotNet8.RefreshTokenSample.Api.AppDbContextModels;
using DotNet8.RefreshTokenSample.Api.Models;
using DotNet8.RefreshTokenSample.Api.Services.SecurityServices;
using DotNet8.RefreshTokenSample.Api.Utils;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.RefreshTokenSample.Api.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly AesService _aesService;

        public UserRepository(AppDbContext context, AesService aesService)
        {
            _context = context;
            _aesService = aesService;
        }

        public async Task<Result<JwtResponseModel>> LoginAsync(LoginRequestModel loginRequest, CancellationToken cs)
        {
            Result<JwtResponseModel> result;
            try
            {
                var user = await _context.Tbl_User.FirstOrDefaultAsync(x => x.Email == loginRequest.Email && x.Password == loginRequest.Password, cancellationToken: cs);
                if (user is null)
                {
                    result = Result<JwtResponseModel>.NotFound();
                    goto result;
                }

                var jwt = GetJwtResponseModel(user);
                result = Result<JwtResponseModel>.Success(jwt);
            }
            catch (Exception ex)
            {
                throw;
            }

        result:
            return result;
        }

        private JwtResponseModel GetJwtResponseModel(Tbl_User tbl_User) => new(
            _aesService.Encrypt(tbl_User.UserId.ToString()),
            _aesService.Encrypt(tbl_User.UserName),
            _aesService.Encrypt(tbl_User.Email));
    }
}
