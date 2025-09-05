namespace PS.FreeBookHub_Lite.OrderService.Application.Security
{
    public interface IAccessTokenProvider
    {
        string? GetAccessToken();
    }
}
