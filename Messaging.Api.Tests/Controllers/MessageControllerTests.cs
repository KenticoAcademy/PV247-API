using System;
using System.Collections.Generic;
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
    public class MessageControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly IMessageRepository _messageRepository;

        public MessageControllerTests(WebApplicationFactory<Startup> factory)
        {
            _messageRepository = Substitute.For<IMessageRepository>();

            _factory = factory.WithWebHostBuilder(builder => builder
                .ConfigureTestServices(services => services.AddScoped(_ => _messageRepository)));
        }

        [Fact]
        public async Task Get_ExistingChannel_Ok()
        {
            var appId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var client = await _factory.CreateAuthenticatedClient();
            _messageRepository.GetAll(appId, channelId, Arg.Any<int>())
                .Returns(new[] {new Message(), new Message()});

            var response = await client.GetAsync($"/api/app/{appId}/channel/{channelId}/message");

            var retrievedMessages = await response.EnsureSuccessStatusCode().Content.ReadAsAsync<List<Message>>();
            Assert.Equal(2, retrievedMessages.Count);
        }

        [Fact]
        public async Task Create_ExistingChannel_Created()
        {
            var appId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var client = await _factory.CreateAuthenticatedClient();
            _messageRepository.Upsert(appId, channelId, Arg.Any<Message>())
                .Returns(call => call.Arg<Message>());

            var response = await client.PostAsync($"/api/app/{appId}/channel/{channelId}/message", new JsonContent(new EditedMessage()));

            var createdMessage = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<Message>();

            Assert.NotEqual(Guid.Empty, createdMessage.Id);
            Assert.Equal($"/api/app/{appId}/channel/{channelId}/message", response.Headers.Location.LocalPath);
        }

        [Fact]
        public async Task Edit_ExistingMessage_Ok()
        {
            var appId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var messageId = Guid.NewGuid();
            var client = await _factory.CreateAuthenticatedClient();
            _messageRepository.Get(appId, channelId, messageId)
                .Returns(new Message {Id = messageId});
            _messageRepository.Upsert(appId, channelId, Arg.Any<Message>())
                .Returns(call => call.Arg<Message>());

            var newValue = "newMessage";
            var newCustomData = "{ json: 42 }";
            var response = await client.PutAsync($"/api/app/{appId}/channel/{channelId}/message/{messageId}", new JsonContent(new EditedMessage
                {
                    Value = newValue,
                    CustomData = newCustomData
                }));

            var editedMessage = await response.EnsureSuccessStatusCode()
                .Content.ReadAsAsync<Message>();

            Assert.Equal(newValue, editedMessage.Value);
            Assert.Equal(newCustomData, editedMessage.CustomData);
        }

        [Fact]
        public async Task Delete_ExistingMessage_Ok()
        {
            var appId = Guid.NewGuid();
            var channelId = Guid.NewGuid();
            var messageId = Guid.NewGuid();
            var client = await _factory.CreateAuthenticatedClient();
            _messageRepository.Delete(appId, channelId, messageId)
                .Returns(true);

            var response = await client.DeleteAsync($"/api/app/{appId}/channel/{channelId}/message/{messageId}");

            response.EnsureSuccessStatusCode();
        }
    }
}
