using System;
using Messaging.Contract.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Messaging.Api.Services
{
    internal class ChannelPatchService : IChannelPatchService
    {
        public JsonPatchDocument<Application> ConvertOperations(JsonPatchDocument<Application> patch, Application application)
        {
            const string collectionPath = "/channels";
            const string itemPathPrefix = collectionPath + "/";

            foreach (var operation in patch.Operations)
            {
                switch (operation.OperationType)
                {
                    case OperationType.Add:
                    case OperationType.Remove:
                    case OperationType.Replace:
                        if (operation.path.Equals(collectionPath, StringComparison.InvariantCultureIgnoreCase))
                            break;

                        if (!operation.path.StartsWith(itemPathPrefix, StringComparison.InvariantCultureIgnoreCase))
                            throw new InvalidOperationException($"Invalid JSON patch path '{operation.path}'.");

                        string itemPointer = operation.path.Substring(itemPathPrefix.Length);
                        if (itemPointer.Equals("-", StringComparison.Ordinal))
                        {
                            if (operation.OperationType != OperationType.Add)
                                throw new InvalidOperationException("Only addition to the end is supported.");

                            if (operation.value is Channel newChannel)
                            {
                                newChannel.Id = Guid.NewGuid();
                            }

                            break;
                        }

                        if (!Guid.TryParse(itemPointer, out var channelId))
                            throw new InvalidOperationException($"Invalid identifier in JSON patch path '{operation.path}'.");

                        var channelIndex = application.Channels
                            .FindIndex(channel => channel.Id == channelId);

                        if (channelIndex < 0)
                            throw new InvalidOperationException("Channel with the specified ID was not found.");

                        if (operation.value is Channel updatedChannel && updatedChannel.Id != channelId)
                            throw new InvalidOperationException("Changing ID of an existing channel is not allowed.");

                        operation.path = itemPathPrefix + channelIndex;
                        break;

                    default:
                        throw new InvalidOperationException($"Invalid JSON patch operation '{operation.op}'.");
                }
            }

            return patch;
        }
    }
}
