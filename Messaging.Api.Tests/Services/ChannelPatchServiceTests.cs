using System;
using System.Collections.Generic;
using System.Linq;
using Messaging.Api.Services;
using Messaging.Contract.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Xunit;

namespace Messaging.Api.Tests.Services
{
    public class ChannelPatchServiceTests
    {
        [Fact]
        public void ConvertOperations_ChangeChannelId_Throws()
        {
            var service = new ChannelPatchService();

            var channelId = Guid.NewGuid();
            var application = new Application
            {
                Channels = new List<Channel>
                {
                    new Channel { Id = channelId }
                }
            };

            var patchDocument = new JsonPatchDocument<Application>();
            patchDocument.Operations.Add(new Operation<Application>("replace", $"/channels/{channelId}", null,
                new Channel
                {
                    Id = Guid.NewGuid()
                }));

            Assert.Throws<InvalidOperationException>(() => service.ConvertOperations(patchDocument, application));
        }

        [Fact]
        public void ConvertOperations_MissingIdInValue_Throws()
        {
            var service = new ChannelPatchService();

            var channelId = Guid.NewGuid();
            var application = new Application
            {
                Channels = new List<Channel>
                {
                    new Channel { Id = channelId }
                }
            };

            var patchDocument = new JsonPatchDocument<Application>();
            patchDocument.Operations.Add(new Operation<Application>("replace", $"/channels/{channelId}", null,
                new Channel
                {
                    Name = "Named channel"
                }));

            Assert.Throws<InvalidOperationException>(() => service.ConvertOperations(patchDocument, application));
        }

        [Fact]
        public void ConvertOperation_ReplaceMissingChannel_Throws()
        {
            var service = new ChannelPatchService();

            var application = new Application
            {
                Channels = new List<Channel>()
            };

            var channelId = Guid.NewGuid();

            var patchDocument = new JsonPatchDocument<Application>();
            patchDocument.Operations.Add(new Operation<Application>("replace", $"/channels/{channelId}", null,
                new Channel
                {
                    Id = channelId
                }));

            Assert.Throws<InvalidOperationException>(() => service.ConvertOperations(patchDocument, application));
        }

        [Fact]
        public void ConvertOperations_AddChannel_GeneratesChannelId()
        {
            var service = new ChannelPatchService();

            var application = new Application
            {
                Channels = new List<Channel>()
            };
            var nonExistingChannelId = Guid.NewGuid();
            var patchDocument = new JsonPatchDocument<Application>();
            patchDocument.Operations.Add(new Operation<Application>("add", "/channels/-", null, new Channel
            {
                Id = nonExistingChannelId
            }));

            var convertedPatch = service.ConvertOperations(patchDocument, application);

            convertedPatch.ApplyTo(application);

            var newChannelId = application.Channels.Single().Id;
            Assert.NotEqual(nonExistingChannelId, newChannelId);
            Assert.NotEqual(Guid.Empty, newChannelId);
        }

        [Fact]
        public void ConvertOperations_RemoveChannel_RemovesOnlyTheSpecified()
        {
            var service = new ChannelPatchService();

            var channelId = Guid.NewGuid();
            var remainingChannelId = Guid.NewGuid();
            var application = new Application
            {
                Channels = new List<Channel>
                {
                    new Channel { Id = channelId },
                    new Channel { Id = remainingChannelId }
                }
            };

            var patchDocument = new JsonPatchDocument<Application>();
            patchDocument.Operations.Add(new Operation<Application>("remove", $"/channels/{channelId}", null));

            var convertedPatch = service.ConvertOperations(patchDocument, application);

            convertedPatch.ApplyTo(application);

            Assert.Equal(remainingChannelId, application.Channels.Single().Id);
        }
    }
}