namespace PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces
{
    public interface IPasswordHasher
    {
        Task<string> Hash(string password);
        Task<bool> Verify(string password, string hash);
    }
}
