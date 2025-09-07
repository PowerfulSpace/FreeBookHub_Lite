namespace PS.OrderService.Application.Security
{
    public interface IAccessTokenProvider
    {
        string? GetAccessToken();
    }
}
