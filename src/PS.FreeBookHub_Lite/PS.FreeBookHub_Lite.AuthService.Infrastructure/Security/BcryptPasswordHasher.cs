using PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces;

namespace PS.FreeBookHub_Lite.AuthService.Infrastructure.Security
{
    public class BcryptPasswordHasher : IPasswordHasher
    {
        public Task<string> Hash(string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Verify(string password, string hash)
        {
            throw new NotImplementedException();
        }
    }
}
