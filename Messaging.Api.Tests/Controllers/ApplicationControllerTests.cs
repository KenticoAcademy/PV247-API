using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Messaging.Api.Tests.Controllers
{
    public class ApplicationControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IApplicationRepository _applicationRepositoryMock;

        public ApplicationControllerTests(WebApplicationFactory<Startup> factory)
        {
            _applicationRepositoryMock = Substitute.For<IApplicationRepository>();

            _factory = factory.WithWebHostBuilder(builder => builder
                .UseSetting("TokenSigningKey", "asdfasdfasdfasdf")
                .ConfigureTestServices(services => services.AddScoped(_ => _applicationRepositoryMock)));
        }

        [Fact]
        public async Task Get_ExistingApp_Ok()
        {
            var client = _factory.CreateClient();
            var appId = Guid.NewGuid();
            _applicationRepositoryMock.Get(appId).Returns(new Application {Id = appId});

            var response = await client.GetAsync($"/api/app/{appId}");

            var responseContent = await response.EnsureSuccessStatusCode()
                .Content.ReadAsStringAsync();

            var retrievedApp = JsonConvert.DeserializeObject<Application>(responseContent);
            Assert.Equal(appId, retrievedApp.Id);
        }

        [Fact]
        public async Task Get_NonExistingApp_NotFound()
        {
            var client = _factory.CreateClient();
            _applicationRepositoryMock.Get(Arg.Any<Guid>()).Returns((Application)null);

            var response = await client.GetAsync($"/api/app/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_Created()
        {
            var client = _factory.CreateClient();
            _applicationRepositoryMock.Upsert(Arg.Any<Application>()).Returns(call => call.Arg<Application>());

            var response = await client.PostAsync("/api/app", null);

            var responseContent = await response.EnsureSuccessStatusCode()
                .Content.ReadAsStringAsync();

            var createdApp = JsonConvert.DeserializeObject<Application>(responseContent);
            Assert.NotEqual(Guid.Empty, createdApp.Id);
        }

        [Fact]
        public async Task Patch_ExistingApp_Ok()
        {
            var appId = Guid.NewGuid();
            var client = await _factory.CreateAuthenticatedClient();
            _applicationRepositoryMock.Get(appId).Returns(new Application {Id = appId, Channels = new List<Channel>{ new Channel() }});
            _applicationRepositoryMock.Upsert(Arg.Any<Application>()).Returns(call => call.Arg<Application>());
            
            var response = await client.SendAsync(new HttpRequestMessage(new HttpMethod("PATCH"), $"/api/app/{appId}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(new[]
                {
                    new {op = "add", path = "/channels/-", value = new Channel()}
                }), Encoding.UTF8, "application/json")
            });

            var responseContent = await response.EnsureSuccessStatusCode()
                .Content.ReadAsStringAsync();

            var patchedApp = JsonConvert.DeserializeObject<Application>(responseContent);

            Assert.Equal(2, patchedApp.Channels.Count);
        }
    }
}
