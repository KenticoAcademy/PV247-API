using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messaging.Api.Models;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Messaging.Api.Tests.Controllers
{
    public class ChannelControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IApplicationRepository _applicationRepository;

        public ChannelControllerTests(WebApplicationFactory<Startup> factory)
        {
            _applicationRepository = Substitute.For<IApplicationRepository>();

            _factory = factory.WithWebHostBuilder(builder => builder
                .ConfigureTestServices(services => services.AddScoped(_ => _applicationRepository)));
        }

        [Fact]
        public async Task GetChannels_Existing_Ok()
        {
            var appId = Guid.NewGuid();
            var client = await _factory.CreateAuthenticatedClient();
            _applicationRepository.Get(appId).Returns(new Application
            {
                Id = appId,
                Channels = new List<Channel>
                {
                    new Channel { Id = Guid.NewGuid(), Name = "B" },
                    new Channel { Id = Guid.NewGuid(), Name = "A" }
                }
            });

            var response = await client.GetAsync($"/api/app/{appId}/channel");

            var retrievedChannels = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<List<Channel>>();
            Assert.Equal(new[] { "A", "B" }, retrievedChannels.Select(channel => channel.Name));
        }

        [Fact]
        public async Task GetChannel_Existing_Ok()
        {
            var appId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var client = await _factory.CreateAuthenticatedClient();
            _applicationRepository.Get(appId).Returns(new Application
            {
                Id = appId,
                Channels = new List<Channel>
                {
                    new Channel { Id = channelId }
                }
            });

            var response = await client.GetAsync($"/api/app/{appId}/channel/{channelId}");

            var retrievedChannel = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<Channel>();
            Assert.Equal(channelId, retrievedChannel.Id);
        }

        [Fact]
        public async Task AddChannel_Created()
        {
            var appId = Guid.NewGuid();
            var channelName = "test";
            var channelCustomData = "{ json: true }";
            var client = await _factory.CreateAuthenticatedClient();
            _applicationRepository.Get(appId).Returns(new Application
            {
                Id = appId,
                Channels = new List<Channel>
                {
                    new Channel { Id = Guid.NewGuid() }
                }
            });
            
            var response = await client.PostAsync($"/api/app/{appId}/channel", new JsonContent(new ChannelUpdate
            {
                Name = channelName,
                CustomData = channelCustomData
            }));

            var createdChannel = await response.EnsureSuccessStatusCode().Content.ReadAsAsync<Channel>();
            Assert.NotEqual(Guid.Empty, createdChannel.Id);
            Assert.Equal(channelName, createdChannel.Name);
            Assert.Equal(channelCustomData, createdChannel.CustomData);
            await _applicationRepository.Received().Upsert(Arg.Is<Application>(app => app.Channels.Count == 2));
        }

        [Fact]
        public async Task UpdateChannel_Existing_Ok()
        {
            var appId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var channelName = "test";
            var channelCustomData = "{ json: true }";
            var client = await _factory.CreateAuthenticatedClient();
            _applicationRepository.Get(appId).Returns(new Application
            {
                Id = appId,
                Channels = new List<Channel>
                {
                    new Channel {Id = Guid.NewGuid()},
                    new Channel {Id = channelId},
                    new Channel {Id = Guid.NewGuid()}
                }
            });

            var response = await client.PutAsync($"/api/app/{appId}/channel/{channelId}", new JsonContent(new ChannelUpdate
            {
                Name = channelName,
                CustomData = channelCustomData
            }));

            var updatedChannel = await response.EnsureSuccessStatusCode().Content.ReadAsAsync<Channel>();
            Assert.Equal(channelId, updatedChannel.Id);
            Assert.Equal(channelName, updatedChannel.Name);
            Assert.Equal(channelCustomData, updatedChannel.CustomData);
            await _applicationRepository.Received().Upsert(Arg.Is<Application>(app =>
                app.Channels.Count == 3
                && app.Channels.Single(channel => channel.Id == channelId).Name == channelName
                && app.Channels.Single(channel => channel.Id == channelId).CustomData == channelCustomData));
        }

		[Fact]
        public async Task DeleteChannel_Existing_NoContent()
		{
		    var appId = Guid.NewGuid();
		    var channelId = Guid.NewGuid();
            var client = await _factory.CreateAuthenticatedClient();
		    _applicationRepository.Get(appId).Returns(new Application
		    {
		        Id = appId,
		        Channels = new List<Channel>
		        {
		            new Channel {Id = Guid.NewGuid()},
		            new Channel {Id = channelId},
		            new Channel {Id = Guid.NewGuid()}
                }
		    });

		    var response = await client.DeleteAsync($"/api/app/{appId}/channel/{channelId}");

		    response.EnsureSuccessStatusCode();
		    await _applicationRepository.Received().Upsert(Arg.Is<Application>(app =>
		        app.Channels.Count == 2 && app.Channels.All(channel =>
		            channel.Id != channelId)));
		}
    }
}