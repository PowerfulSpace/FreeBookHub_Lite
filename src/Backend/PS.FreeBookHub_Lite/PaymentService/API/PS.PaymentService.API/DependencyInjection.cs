using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PS.PaymentService.API.Authentication.Enums;
using PS.PaymentService.API.Authentication.Models;
using PS.PaymentService.API.Security;
using System.Text;

namespace PS.PaymentService.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
               .AddAuthenticationBearer(configuration)
               .AddAuthorizationPolicies()
               .AddSwaggerSetting()
               .AddJwtSettings(configuration);

            services.AddHttpContextAccessor();

            services.AddSingleton<IAuthorizationHandler, InternalApiKeyHandler>();

            services.AddControllers();

            return services;
        }

        private static IServiceCollection AddAuthenticationBearer(this IServiceCollection services, ConfigurationManager configuration)
        {
            var jwtSettings = configuration.GetSection("Auth:JwtSettings").Get<JwtSettings>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings!.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        };
                    });

            return services;
        }

        private static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy =>
                    policy.RequireRole(
                        UserRole.User.ToString(),
                        UserRole.Moderator.ToString(),
                        UserRole.Admin.ToString()));

                options.AddPolicy("Moderator", policy =>
                    policy.RequireRole(
                        UserRole.Moderator.ToString(),
                        UserRole.Admin.ToString()));

                options.AddPolicy("Admin", policy =>
                    policy.RequireRole(UserRole.Admin.ToString()));

                options.AddPolicy("InternalOnly", policy =>
                {
                    policy.RequireAuthenticatedUser()
                        .AddRequirements(new InternalApiKeyRequirement());
                });

            });

            return services;
        }

        private static IServiceCollection AddSwaggerSetting(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FreeBookHub Payment API",
                    Version = "v1",
                    Description = "API для обработки платежей"
                });

                options.EnableAnnotations();

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
            });

            return services;
        }

        private static IServiceCollection AddJwtSettings(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("Auth:JwtSettings"));

            return services;
        }
    }
}
