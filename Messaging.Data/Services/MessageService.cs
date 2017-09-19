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

        public async Task<IEnumerable<Message>> GetAll(string currentUser, Guid appId, Guid channelId, int lastN)
        {
            // TODO: Check the user is invited to the application?

            return await _messageRepository.GetAll(appId, channelId, lastN);
        }

        public async Task<Message> Create(string currentUser, Guid appId, Guid channelId, Message message)
        {
            message.Id = Guid.NewGuid();
            message.CreatedAt = DateTime.UtcNow;
            message.CreatedBy = currentUser;

            return await _messageRepository.Upsert(appId, channelId, message);
        }

        public async Task<Message> Edit(string currentUser, Guid appId, Guid channelId, Message message)
        {
            message.UpdatedAt = DateTime.UtcNow;
            message.UpdatedBy = currentUser;

            return await _messageRepository.Upsert(appId, channelId, message);
        }

        public async Task<bool> Delete(string currentUser, Guid appId, Guid channelId, Guid messageId)
        {
            // TODO: Check the author is deleting his own message

            return await _messageRepository.Delete(appId, channelId, messageId);
        }
    }
}