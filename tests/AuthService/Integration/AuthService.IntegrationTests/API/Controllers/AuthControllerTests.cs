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
    }
}