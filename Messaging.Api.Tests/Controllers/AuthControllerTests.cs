using System.Net;
using System.Threading.Tasks;
using Messaging.Api.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Messaging.Api.Tests.Controllers
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IUserRepository _userRepositoryMock;

        public AuthControllerTests(WebApplicationFactory<Startup> factory)
        {
            _userRepositoryMock = Substitute.For<IUserRepository>();

            _factory = factory.WithWebHostBuilder(builder => builder
                .UseSetting("TokenSigningKey", "asdfasdfasdfasdf")
                .ConfigureTestServices(services => services.AddScoped(_ => _userRepositoryMock)));
        }

        [Fact]
        public async Task Login_ExistingUser_Ok()
        {
            var email = "test@test.test";
            var client = _factory.CreateClient();
            _userRepositoryMock.IsValidUser(email)
                .Returns(true);

            var response = await client.PostAsync("/api/v2/auth", new JsonContent(new { email }));

            var token = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<LoginResponse>();

            Assert.NotNull(token);
        }

        [Fact]
        public async Task Login_NonExistingUser_BadRequest()
        {
            var client = _factory.CreateClient();
            _userRepositoryMock.IsValidUser(Arg.Any<string>())
                .Returns(false);

            var response = await client.PostAsync("/api/v2/auth", new JsonContent(new {Email = "test@test.test"}));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
