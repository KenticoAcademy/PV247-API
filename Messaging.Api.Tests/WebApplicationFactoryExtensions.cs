using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Messaging.Api.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Messaging.Api.Tests
{
    internal static class WebApplicationFactoryExtensions
    {
        public static async Task<HttpClient> CreateAuthenticatedClient(this WebApplicationFactory<Startup> factory,
            IUserRepository userRepositoryMock = null, string emailToAuthenticate = "current@user.test")
        {
            var userRepository = userRepositoryMock ?? Substitute.For<IUserRepository>();
            userRepository.IsValidUser(emailToAuthenticate).Returns(true);

            var client = factory
                .WithWebHostBuilder(builder => builder
                    .UseSetting("TokenSigningKey", "asdfasdfasdfasdf")
                    .ConfigureTestServices(services => services.AddScoped(_ => userRepository)))
                .CreateClient();

            var response = await client.PostAsync("/api/v2/auth", new JsonContent(new LoginCredentials {Email = emailToAuthenticate}));
            var token = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<LoginResponse>();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            return client;
        }
    }
}