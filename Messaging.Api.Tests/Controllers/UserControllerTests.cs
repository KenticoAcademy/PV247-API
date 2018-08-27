using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Messaging.Api.ViewModels;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Messaging.Api.Tests.Controllers
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly IUserRepository _userRepository;
        private readonly WebApplicationFactory<Startup> _factory;

        public UserControllerTests(WebApplicationFactory<Startup> factory)
        {
            _userRepository = Substitute.For<IUserRepository>();
            _factory = factory.WithWebHostBuilder(builder => builder
                .UseSetting("TokenSigningKey", "asdfasdfasdfasdf")
                .ConfigureTestServices(services => services.AddScoped(_ => _userRepository)));
        }

        [Fact]
        public async Task GetApplicationUsers_Existing_Ok()
        {
            var appId = Guid.NewGuid();
            _userRepository.Get(appId)
                .Returns(new[] { new User(), new User(), new User() });

            var client = await _factory.CreateAuthenticatedClient(_userRepository);
            
            var response = await client.GetAsync($"/api/{appId}/user");

            var users = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<List<User>>();

            Assert.Equal(3, users.Count);
        }

        [Fact]
        public async Task Get_ExistingUser_Ok()
        {
            var appId = Guid.NewGuid();
            var email = "test@test.test";
            var client = await _factory.CreateAuthenticatedClient(_userRepository);

            _userRepository.Get(appId, email)
                .Returns(new User { Email = email });

            var response = await client.GetAsync($"/api/{appId}/user/{email}");

            var retrievedUser = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<User>();

            Assert.NotNull(retrievedUser);
        }

        [Fact]
        public async Task Register_NonExistingUser_Created()
        {
            var appId = Guid.NewGuid();
            string email = "test@test.test";
            var client = _factory.CreateClient();

            _userRepository.Upsert(appId, Arg.Any<User>())
                .Returns(call => call.Arg<User>());

            var response = await client.PostAsync($"/api/{appId}/user", new JsonContent(new RegisteredUser
                {
                    Email = email,
                    CustomData = "{ json: true }"
                }));

            response.EnsureSuccessStatusCode();

            Assert.Equal($"/api/{appId}/user/{WebUtility.UrlEncode(email)}", response.Headers.Location.LocalPath);

            var createdUser = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<User>();

            Assert.NotNull(createdUser);
        }

        [Fact]
        public async Task Register_ExistingUser_BadRequest()
        {
            var appId = Guid.NewGuid();
            string email = "test@test.test";
            var client = _factory.CreateClient();

            _userRepository.Get(appId, email)
                .Returns(new User {Email = email});

            var response = await client.PostAsync($"/api/{appId}/user", new JsonContent(new RegisteredUser { Email = email }));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            await _userRepository.DidNotReceiveWithAnyArgs().Upsert(appId, Arg.Any<User>());
        }

        [Fact]
        public async Task Update_Self_Ok()
        {
            var appId = Guid.NewGuid();
            var email = "test@test.test";
            var client = await _factory.CreateAuthenticatedClient(_userRepository, email);

            _userRepository.Get(appId, email)
                .Returns(new User {Email = email});
            _userRepository.Upsert(appId, Arg.Any<User>())
                .Returns(call => call.Arg<User>());

            var response = await client.PutAsync($"/api/{appId}/user/{email}", new StringContent("\"{ json: true }\"", Encoding.UTF8, "application/json"));

            var updatedUser = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<User>();

            Assert.NotNull(updatedUser);
        }

        [Fact]
        public async Task Update_OtherExistingUser_Forbidden()
        {
            var appId = Guid.NewGuid();
            var email = "test@test.test";
            var client = await _factory.CreateAuthenticatedClient(_userRepository);

            _userRepository.Get(appId, email)
                .Returns(new User { Email = email });

            var response = await client.PutAsync($"/api/{appId}/user/{email}", new StringContent("\"{ json: true }\"", Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

            await _userRepository.DidNotReceiveWithAnyArgs().Upsert(appId, Arg.Any<User>());
        }

        [Fact]
        public async Task Update_NonExistingSelf_NotFound()
        {
            // This case is somewhat far-fetched, but authentication and retrieval use different repo methods.
            var appId = Guid.NewGuid();
            var email = "test@test.test";
            var client = await _factory.CreateAuthenticatedClient(_userRepository, email);

            var response = await client.PutAsync($"/api/{appId}/user/{email}", new StringContent("\"{ json: true }\"", Encoding.UTF8, "application/json"));

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
