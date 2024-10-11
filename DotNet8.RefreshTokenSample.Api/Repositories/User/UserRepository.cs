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
