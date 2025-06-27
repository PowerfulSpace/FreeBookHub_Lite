using Mapster;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Login;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Logout;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.RefreshToken;
using PS.FreeBookHub_Lite.AuthService.Application.CQRS.Commands.Register;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace PS.FreeBookHub_Lite.AuthService.Application.Mappings
{
    public class AuthMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, UserDto>()
                .Map(dest => dest.Role, src => src.Role.ToString());

            config.NewConfig<RegisterUserRequest, RegisterCommand>();

            config.NewConfig<LoginRequest, LoginCommand>();

            config.NewConfig<RefreshTokenRequest, RefreshTokenCommand>();

            config.NewConfig<LogoutRequest, LogoutCommand>();
        }
    }
}
