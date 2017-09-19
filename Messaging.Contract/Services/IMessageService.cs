using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messaging.Contract.Models;

namespace Messaging.Contract.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetAll(string currentUser, Guid appId, Guid channelId, int lastN);

        Task<Message> Create(string currentUser, Guid appId, Guid channelId, Message message);

        Task<Message> Edit(string currentUser, Guid appId, Guid channelId, Message message);

        Task<bool> Delete(string currentUser, Guid appId, Guid channelId, Guid messageId);
    }
}