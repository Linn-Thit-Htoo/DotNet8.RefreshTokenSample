using DotNet8.RefreshTokenSample.Api.AppDbContextModels;
using DotNet8.RefreshTokenSample.Api.Repositories.Jwt;
using DotNet8.RefreshTokenSample.Api.Repositories.User;
using DotNet8.RefreshTokenSample.Api.Services.AuthServices;
using DotNet8.RefreshTokenSample.Api.Services.SecurityServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DotNet8.RefreshTokenSample.Api.Dependencies
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, WebApplicationBuilder builder)
        {
            return services.AddDbContextService(builder).AddDataAccessService()
                .AddAuthenticationService(builder)
                .AddCustomServices();
        }

        private static IServiceCollection AddDbContextService(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Transient, ServiceLifetime.Transient);

            return services;
        }

        private static IServiceCollection AddDataAccessService(this IServiceCollection services)
        {
            return services.AddScoped<IUserRepository, UserRepository>().AddScoped<IJWTManagerRepository, JWTManagerRepository>();
        }

        private static IServiceCollection AddAuthenticationService(
    this IServiceCollection services,
    WebApplicationBuilder builder
)
        {
            builder
                .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                        )
                    };
                });

            return services;
        }

        private static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            return services.AddTransient<TokenValidationService>().AddScoped<AesService>();
        }
    }
}
