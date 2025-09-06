namespace PS.FreeBookHub_Lite.AuthService.Application.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
