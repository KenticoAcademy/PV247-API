using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            _userRepositoryMock.IsValidUser(email).Returns(true);

            var response = await client.PostAsync("/api/auth", new StringContent($"\"{email}\"", Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Login_NonExistingUser_BadRequest()
        {
            var client = _factory.CreateClient();
            _userRepositoryMock.IsValidUser(Arg.Any<string>()).Returns(false);

            var response = await client.PostAsync("/api/auth", new StringContent($"\"test@test.test\"", Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
