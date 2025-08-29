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

        // --- Happy path тесты (у тебя уже были) ---

        [Fact]
        public async Task Register_Should_Return_Tokens()
        {
            var request = new RegisterUserRequest
            {
                Email = "newuser@example.com",
                Password = "securePassword123"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(result.RefreshToken));
        }

        [Fact]
        public async Task Login_Should_Return_Tokens_For_Valid_Credentials()
        {
            var registerRequest = new RegisterUserRequest
            {
                Email = "loginuser@example.com",
                Password = "Secure123!"
            };

            await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = "loginuser@example.com",
                Password = "Secure123!"
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            loginResponse.EnsureSuccessStatusCode();
            var tokens = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

            Assert.NotNull(tokens);
            Assert.False(string.IsNullOrWhiteSpace(tokens!.AccessToken));
            Assert.False(string.IsNullOrWhiteSpace(tokens!.RefreshToken));
        }

        [Fact]
        public async Task Refresh_Should_Return_New_Tokens_When_Valid()
        {
            var registerRequest = new RegisterUserRequest
            {
                Email = "refreshuser@example.com",
                Password = "MyPassword123!"
            };

            await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

            var loginRequest = new LoginRequest
            {
                Email = registerRequest.Email,
                Password = registerRequest.Password
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var tokens = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

            var refreshRequest = new RefreshTokenRequest { RefreshToken = tokens.RefreshToken };
            var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh", refreshRequest);

            refreshResponse.EnsureSuccessStatusCode();
            var refreshedTokens = await refreshResponse.Content.ReadFromJsonAsync<AuthResponse>();

            Assert.NotNull(refreshedTokens);
            Assert.NotEqual(tokens.AccessToken, refreshedTokens!.AccessToken);
            Assert.NotEqual(tokens.RefreshToken, refreshedTokens.RefreshToken);
            Assert.True(refreshedTokens.ExpiresAt > tokens.ExpiresAt);

            _client.DefaultRequestHeaders.Authorization = null; // сбрасываем
        }

        [Fact]
        public async Task Logout_Should_Revoke_Refresh_Token()
        {
            var email = "logoutuser@example.com";
            var password = "MyPassword123!";

            await _client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest
            {
                Email = email,
                Password = password
            });

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = password
            });

            var tokens = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
            _client.DefaultRequestHeaders.Authorization = new("Bearer", tokens!.AccessToken);

            var logoutRequest = new LogoutRequest { RefreshToken = tokens.RefreshToken };
            var logoutResponse = await _client.PostAsJsonAsync("/api/auth/logout", logoutRequest);

            Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

            var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest
            {
                RefreshToken = tokens.RefreshToken
            });

            Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);

            _client.DefaultRequestHeaders.Authorization = null;
        }

        [Fact]
        public async Task LogoutAll_Should_Revoke_All_Refresh_Tokens_For_User()
        {
            var email = "logoutalluser@example.com";
            var password = "MyPassword123!";

            await _client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest
            {
                Email = email,
                Password = password
            });

            var loginResponse1 = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = password
            });

            var tokens1 = await loginResponse1.Content.ReadFromJsonAsync<AuthResponse>();

            var loginResponse2 = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = password
            });

            var tokens2 = await loginResponse2.Content.ReadFromJsonAsync<AuthResponse>();

            _client.DefaultRequestHeaders.Authorization = new("Bearer", tokens1!.AccessToken);

            var logoutAllResponse = await _client.PostAsync("/api/auth/logout-all", null);

            Assert.Equal(HttpStatusCode.NoContent, logoutAllResponse.StatusCode);

            var failedRefresh1 = await _client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest
            {
                RefreshToken = tokens1.RefreshToken
            });

            var failedRefresh2 = await _client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest
            {
                RefreshToken = tokens2!.RefreshToken
            });

            Assert.Equal(HttpStatusCode.Unauthorized, failedRefresh1.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, failedRefresh2.StatusCode);

            _client.DefaultRequestHeaders.Authorization = null;
        }

        // --- Дополнительные тесты (негативные сценарии) ---

        [Fact]
        public async Task Register_Should_Fail_When_Email_Already_Exists()
        {
            var request = new RegisterUserRequest
            {
                Email = "duplicate@example.com",
                Password = "Password123!"
            };

            await _client.PostAsJsonAsync("/api/auth/register", request);

            var secondResponse = await _client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
        }

        [Fact]
        public async Task Login_Should_Fail_For_Invalid_Password()
        {
            var email = "wrongpass@example.com";
            var password = "Password123!";

            await _client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest
            {
                Email = email,
                Password = password
            });

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = email,
                Password = "WrongPassword!"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
        }

        [Fact]
        public async Task Login_Should_Fail_For_Unknown_Email()
        {
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = "notexist@example.com",
                Password = "SomePassword123!"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
        }

        [Fact]
        public async Task Refresh_Should_Fail_For_Invalid_Token()
        {
            var refreshResponse = await _client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokenRequest
            {
                RefreshToken = "invalid-refresh-token"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, refreshResponse.StatusCode);
        }

        [Fact]
        public async Task Logout_Should_Fail_For_Invalid_Token()
        {
            var logoutResponse = await _client.PostAsJsonAsync("/api/auth/logout", new LogoutRequest
            {
                RefreshToken = "invalid-refresh-token"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, logoutResponse.StatusCode);
        }
    }
}