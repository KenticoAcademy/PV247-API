using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> Get(Guid appId, Guid channelId, Guid messageId);

        Task<IEnumerable<Message>> GetAll(Guid appId, Guid channelId, int lastN);

        Task<Message> Upsert(Guid appId, Guid channelId, Message message);

        Task<bool> Delete(Guid appId, Guid channelId, Guid messageId);
    }
}