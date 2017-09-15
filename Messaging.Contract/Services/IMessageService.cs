using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAll(Guid appId, Guid channelId, int lastN);

        Task<Message> Create(Guid appId, Guid channelId, Message message);

        Task<Message> Edit(Guid appId, Guid channelId, Message message);

        Task<bool> Delete(Guid appId, Guid channelId, Guid messageId);
    }
}