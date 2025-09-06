namespace PS.FreeBookHub_Lite.CartService.Application.Security
{
    public interface IAccessTokenProvider
    {
        string? GetAccessToken();
    }
}
