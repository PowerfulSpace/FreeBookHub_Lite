namespace PS.CartService.Application.Security
{
    public interface IAccessTokenProvider
    {
        string? GetAccessToken();
    }
}
