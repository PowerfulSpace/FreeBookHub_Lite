using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.FreeBookHub_Lite.AuthService.Application.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Autentication;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence.Repositories;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Security;

namespace PS.FreeBookHub_Lite.AuthService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddPersistance(configuration)
                .AddJwtSettings(configuration);

            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

        private static IServiceCollection AddPersistance(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AuthDb")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            return services;
        }

        private static IServiceCollection AddJwtSettings(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("Auth:JwtSettings"));

            return services;
        }
    }
}