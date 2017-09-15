using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Messaging.Contract.Services;

namespace Messaging.Data.Services
{
    internal class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<IEnumerable<Message>> GetAll(Guid appId, Guid channelId, int lastN)
        {
            return await _messageRepository.GetAll(appId, channelId, lastN);
        }

        public async Task<Message> Create(Guid appId, Guid channelId, Message message)
        {
            message.Id = Guid.NewGuid();
            message.CreatedAt = DateTime.UtcNow;
            // message.CreatedBy = "TODO";

            return await _messageRepository.Upsert(appId, channelId, message);
        }

        public async Task<Message> Edit(Guid appId, Guid channelId, Message message)
        {
            message.UpdatedAt = DateTime.UtcNow;
            // message.UpdatedBy = "TODO";

            return await _messageRepository.Upsert(appId, channelId, message);
        }

        public async Task<bool> Delete(Guid appId, Guid channelId, Guid messageId)
        {
           return await _messageRepository.Delete(appId, channelId, messageId);
        }
    }
}