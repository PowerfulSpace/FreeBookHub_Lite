using AuthService.IntegrationTests.TestUtils.Factories;
using PS.FreeBookHub_Lite.AuthService.Application.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace AuthService.IntegrationTests.API.Controllers
{
    public class AuthControllerTests : IClassFixture<AuthApiFactory>
    {
        private readonly HttpClient _client;

        public AuthControllerTests(AuthApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_Should_Return_Tokens()
        {
            // Arrange
            var request = new RegisterUserRequest
            {
                Email = "newuser@example.com",
                Password = "securePassword123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        }

        [Fact]
        public async Task Login_Should_Return_Tokens_For_Valid_Credentials()
        {
            // Arrange
            var factory = new AuthApiFactory();
            var client = factory.CreateClient();

            var registerRequest = new RegisterUserRequest
            {
                Email = "loginuser@example.com",
                Password = "Secure123!"
            };

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
            registerResponse.EnsureSuccessStatusCode();

            var loginRequest = new LoginRequest
            {
                Email = "loginuser@example.com",
                Password = "Secure123!"
            };

            // Act
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            loginResponse.EnsureSuccessStatusCode();
            var tokens = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

            Assert.NotNull(tokens);
            Assert.False(string.IsNullOrWhiteSpace(tokens!.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(tokens!.RefreshToken));
        }

        [Fact]
        public async Task Refresh_Should_Return_New_Tokens_When_Valid()
        {
            // Arrange
            var factory = new AuthApiFactory();
            var client = factory.CreateClient();

            // Register
            var registerRequest = new RegisterUserRequest
            {
                Email = "refreshuser@example.com",
                Password = "MyPassword123!"
            };

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);
            registerResponse.EnsureSuccessStatusCode();

            // Login
            var loginRequest = new LoginRequest
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password
            };

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();

            var tokens = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

            // Устанавливаем access token в заголовок
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

            // Act — Refresh
            var refreshRequest = new RefreshTokenRequest
            {
                RefreshToken = tokens.RefreshToken
            };

            var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

            // Assert
            refreshResponse.EnsureSuccessStatusCode();
            var refreshedTokens = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();

            Assert.NotNull(refreshedTokens);
            Assert.NotEqual(tokens.AccessToken, refreshedTokens!.AccessToken);
            Assert.NotEqual(tokens.RefreshToken, refreshedTokens.RefreshToken);
            Assert.True(refreshedTokens.ExpiresAt > tokens.ExpiresAt);
        }

        [Fact]
        public async Task Logout_Should_Revoke_Refresh_Token()
        {
            // Arrange
            var factory = new AuthApiFactory();
            var client = factory.CreateClient();

            var email = "logoutuser@example.com";
            var password = "MyPassword123!";

            await client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest
            {
                Email = email,
                Password = password
            });

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = password
            });

            var tokens = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            client.DefaultRequestHeaders.Authorization = new("Bearer", tokens!.AccessToken);

            // Act — Logout
            var logoutRequest = new LogoutRequest
            {
                RefreshToken = tokens.RefreshToken
            };

            var logoutResponse = await client.PostAsJsonAsync("/api/auth/logout", logoutRequest);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

            // Попытка refresh — должна провалиться
            var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest
            {
                RefreshToken = tokens.RefreshToken
            });

            Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
        }

        [Fact]
        public async Task LogoutAll_Should_Revoke_All_Refresh_Tokens_For_User()
        {
            // Arrange
            var factory = new AuthApiFactory();
            var client = factory.CreateClient();

            var email = "logoutalluser@example.com";
            var password = "MyPassword123!";

            await client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest
            {
                Email = email,
                Password = password
            });

            var loginResponse1 = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = password
            });

            var tokens1 = await loginResponse1.Content.ReadFromJsonAsync<AuthResponse>();

            // эмулируем вторую сессию
            var loginResponse2 = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = password
            });

            var tokens2 = await loginResponse2.Content.ReadFromJsonAsync<AuthResponse>();

            client.DefaultRequestHeaders.Authorization = new("Bearer", tokens1!.AccessToken);

            // Act — LogoutAll
            var logoutAllResponse = await client.PostAsync("/api/auth/logout-all", null);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, logoutAllResponse.StatusCode);

            // Проверим, что оба refresh токена более невалидны
            var failedRefresh1 = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest
            {
                RefreshToken = tokens1.RefreshToken
            });

            var failedRefresh2 = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest
            {
                RefreshToken = tokens2!.RefreshToken
            });

            Assert.Equal(HttpStatusCode.Unauthorized, failedRefresh1.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, failedRefresh2.StatusCode);
        }
    }
}