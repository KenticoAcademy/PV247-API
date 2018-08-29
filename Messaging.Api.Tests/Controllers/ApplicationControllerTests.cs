using System;
using System.Net;
using System.Threading.Tasks;
using Messaging.Api.Models;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
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
            _applicationRepositoryMock.Get(appId)
                .Returns(new Application {Id = appId});

            var response = await client.GetAsync($"/api/app/{appId}");

            var retrievedApp = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<ApplicationResponse>();
            Assert.Equal(appId, retrievedApp.Id);
        }

        [Fact]
        public async Task Get_NonExistingApp_NotFound()
        {
            var client = _factory.CreateClient();
            _applicationRepositoryMock.Get(Arg.Any<Guid>())
                .Returns((Application)null);

            var response = await client.GetAsync($"/api/app/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Create_Created()
        {
            var client = _factory.CreateClient();
            _applicationRepositoryMock.Upsert(Arg.Any<Application>())
                .Returns(call => call.Arg<Application>());

            var customData = JObject.FromObject(new { json = true });
            var response = await client.PostAsync("/api/app", new JsonContent(new ApplicationUpdate
            {
                CustomData = customData
            }));

            var createdApp = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<ApplicationResponse>();
            Assert.NotEqual(Guid.Empty, createdApp.Id);
            Assert.Equal(customData, createdApp.CustomData);
        }

        [Fact]
        public async Task Update_ExistingApp_Ok()
        {
            var client = await _factory.CreateAuthenticatedClient();
            var appId = Guid.NewGuid();
            _applicationRepositoryMock.Get(appId)
                .Returns(new Application { Id = appId });
            _applicationRepositoryMock.Upsert(Arg.Any<Application>())
                .Returns(call => call.Arg<Application>());

            var newCustomData = JObject.FromObject(new { json = true });
            var response = await client.PutAsync($"/api/app/{appId}", new JsonContent(new ApplicationUpdate
            {
                CustomData = newCustomData
            }));

            var updatedApp = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<ApplicationResponse>();
            Assert.Equal(appId, updatedApp.Id);
            Assert.Equal(newCustomData, updatedApp.CustomData);
        }
    }
}
