using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Messaging.Data.Models;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data.Repositories
{
    internal class MessageRepository : IMessageRepository
    {
        private const string RowKeyPrefix = "M;";
        private readonly CloudTable _table;

        public MessageRepository(TableClientFactory clientFactory)
        {
            _table = clientFactory.GetTableClient()
                .GetTableReference("DataTable");
        }

        private TableQuery<T> GetWithinPartitionStartsWithByRowKeyQuery<T>(string partitionKey, string startsWithPattern) where T : ITableEntity, new()
        {
            var query = new TableQuery<T>();

            var length = startsWithPattern.Length - 1;
            var lastChar = startsWithPattern[length];

            var nextLastChar = (char) (lastChar + 1);

            var startsWithEndPattern = startsWithPattern.Substring(0, length) + nextLastChar;

            var prefixCondition = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("RowKey",
                    QueryComparisons.GreaterThanOrEqual,
                    startsWithPattern),
                TableOperators.And,
                TableQuery.GenerateFilterCondition("RowKey",
                    QueryComparisons.LessThan,
                    startsWithEndPattern)
            );

            var filterString = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal,
                    partitionKey),
                TableOperators.And,
                prefixCondition
            );

            return query.Where(filterString);
        }

        private async Task<MessageEntity> GetEntity(Guid appId, Guid channelId, Guid messageId)
        {
            var existingResult = await _table.ExecuteAsync(TableOperation.Retrieve<MessageEntity>(appId.ToString(), GetRowKey(channelId, messageId)));
            return (MessageEntity)existingResult.Result;
        }

        public async Task<Message> Get(Guid appId, Guid channelId, Guid messageId)
        {
            var message = await GetEntity(appId, channelId, messageId);

            return message == null ? null : ToDto(message);
        }

        public async Task<IEnumerable<Message>> GetAll(Guid appId, Guid channelId, int lastN)
        {
            var query = GetWithinPartitionStartsWithByRowKeyQuery<MessageEntity>(appId.ToString(), RowKeyPrefix + channelId);
            TableContinuationToken continuationToken = null;
            var items = new List<Message>();
            do
            {
                var segment = await _table.ExecuteQuerySegmentedAsync(query, continuationToken);
                if (segment != null)
                {
                    continuationToken = segment.ContinuationToken;
                    items.AddRange(segment.Select(ToDto));
                }
            } while (continuationToken != null);

            return items;
        }

        public async Task<Message> Upsert(Guid appId, Guid channelId, Message message)
        {
            var entity = new MessageEntity
            {
                PartitionKey = appId.ToString(),
                RowKey = GetRowKey(channelId, message.Id),
                Value = message.Value,
                CreatedAt = message.CreatedAt,
                CreatedBy = message.CreatedBy,
                UpdatedAt = message.CreatedAt,
                UpdatedBy = message.CreatedBy,
                CustomData = message.CustomData
            };
            var result = await _table.ExecuteAsync(TableOperation.Insert(entity));
            var updated = (MessageEntity)result.Result;

            return ToDto(updated);
        }

        public async Task<bool> Delete(Guid appId, Guid channelId, Guid messageId)
        {
            var existing = await GetEntity(appId, channelId, messageId);
            if (existing == null)
                return false;

            await _table.ExecuteAsync(TableOperation.Delete(existing));

            return true;
        }

        private static string GetRowKey(Guid channelId, Guid messageId) => $"{RowKeyPrefix}{channelId};{messageId}";

        private static Message ToDto(MessageEntity entity) => new Message
        {
            Id = Guid.Parse(entity.RowKey.Split(';').Last()),
            Value = entity.Value,
            CreatedAt = entity.CreatedAt,
            CreatedBy = entity.CreatedBy,
            UpdatedAt = entity.UpdatedAt,
            UpdatedBy = entity.UpdatedBy,
            CustomData = entity.CustomData
        };
    }
}