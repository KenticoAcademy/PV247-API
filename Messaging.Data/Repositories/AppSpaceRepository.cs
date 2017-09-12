using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messaging.Contract.Models;
using Messaging.Contract.Repositories;
using Messaging.Data.Models;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Messaging.Data.Repositories
{
    internal class AppSpaceRepository : IAppSpaceRepository
    {
        private const string AppSpaceRowKey = "App";

        private readonly CloudTable _table;

        public AppSpaceRepository(TableClientFactory clientFactory)
        {
            _table = clientFactory.GetTableClient()
                .GetTableReference("DataTable");
        }

        public async Task<AppSpace> Get(Guid appId)
        {
            var result = await _table.ExecuteAsync(TableOperation.Retrieve<AppSpaceEntity>(appId.ToString(), AppSpaceRowKey));
            var entity = (AppSpaceEntity) result.Result;

            return ToDto(entity);
        }

        public async Task<AppSpace> Upsert(AppSpace app)
        {
            var entity = new AppSpaceEntity
            {
                PartitionKey = app.Id.ToString(),
                RowKey = AppSpaceRowKey,
                ChannelsJson = JsonConvert.SerializeObject(app.Channels)
            };

            var result = await _table.ExecuteAsync(TableOperation.InsertOrReplace(entity));
            var updatedEntity = (AppSpaceEntity) result.Result;

            return ToDto(updatedEntity);
        }

        private static AppSpace ToDto(AppSpaceEntity entity)
        {
            return new AppSpace
            {
                Id = Guid.Parse(entity.PartitionKey),
                Channels = JsonConvert.DeserializeObject<ICollection<Channel>>(entity.ChannelsJson)
            };
        }
    }
}