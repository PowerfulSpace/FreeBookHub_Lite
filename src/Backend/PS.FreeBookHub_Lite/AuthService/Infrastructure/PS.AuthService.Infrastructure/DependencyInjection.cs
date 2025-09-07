using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PS.AuthService.Application.Interfaces;
using PS.AuthService.Application.Services.Interfaces;
using PS.AuthService.Infrastructure.Autentication;
using PS.AuthService.Infrastructure.Persistence;
using PS.AuthService.Infrastructure.Persistence.Repositories;
using PS.AuthService.Infrastructure.Security;

namespace PS.AuthService.Infrastructure
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


            //Нужен для интеграционных тестов, временное решение
            //services.AddDbContext<AuthDbContext>(options =>
            //{
            //    options.UseSqlite("DataSource=:memory:");
            //});

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