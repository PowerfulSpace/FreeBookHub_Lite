using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PS.FreeBookHub_Lite.AuthService.API.Filters;
using PS.FreeBookHub_Lite.AuthService.Domain.Enums;
using PS.FreeBookHub_Lite.AuthService.Infrastructure.Autentication;
using System.Text;

namespace PS.FreeBookHub_Lite.AuthService.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddAuthenticationBearer(configuration)
                .AddAuthorizationPolicies()
                .AddSwagerSetting();

            services.AddAuthorization();

            services.AddControllers();

            return services;
        }

        private static IServiceCollection AddAuthenticationBearer(this IServiceCollection services, ConfigurationManager configuration)
        {

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var jwtSettings = configuration.GetSection("Auth:JwtSettings").Get<JwtSettings>();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings!.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                    };
                });

            return services;
        }

        private static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Политика для обычных пользователей
                options.AddPolicy("User", policy =>
                    policy.RequireRole(
                        UserRole.User.ToString(),
                        UserRole.Moderator.ToString(),
                        UserRole.Admin.ToString())
                    );

                // Политика для модераторов
                options.AddPolicy("Moderator", policy =>
                    policy.RequireRole(
                        UserRole.Moderator.ToString(),
                        UserRole.Admin.ToString())
                    );

                // Политика только для админов
                options.AddPolicy("Admin", policy =>
                    policy.RequireRole(
                        UserRole.Admin.ToString())
                    );
            });

            return services;
        }

        private static IServiceCollection AddSwagerSetting(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer(); // Обязательно

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FreeBookHub Auth API",
                    Version = "v1",
                    Description = "API для аутентификации пользователей"
                });

                options.EnableAnnotations();

                // JWT авторизация
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите JWT токен в формате: Bearer {ваш токен}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });


            return services;
        }
    }
}
