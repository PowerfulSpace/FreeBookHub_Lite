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

       
    }
}