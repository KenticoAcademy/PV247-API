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
    internal class ApplicationRepository : IApplicationRepository
    {
        private const string ApplicationRowKey = "App";

        private readonly CloudTable _table;

        public ApplicationRepository(StorageClientFactory clientFactory)
        {
            _table = clientFactory.GetTableClient()
                .GetTableReference("DataTable");
        }

        public async Task<Application> Get(Guid appId)
        {
            var result = await _table.ExecuteAsync(TableOperation.Retrieve<ApplicationEntity>(appId.ToString(), ApplicationRowKey));
            var entity = (ApplicationEntity) result.Result;

            return ToDto(entity);
        }

        public async Task<Application> Upsert(Application app)
        {
            var entity = new ApplicationEntity
            {
                PartitionKey = app.Id.ToString(),
                RowKey = ApplicationRowKey,
                CustomDataJson = app.CustomData,
                ChannelsJson = JsonConvert.SerializeObject(app.Channels)
            };

            var result = await _table.ExecuteAsync(TableOperation.InsertOrReplace(entity));
            var updatedEntity = (ApplicationEntity) result.Result;

            return ToDto(updatedEntity);
        }

        private static Application ToDto(ApplicationEntity entity)
        {
            return new Application
            {
                Id = Guid.Parse(entity.PartitionKey),
                CustomData = entity.CustomDataJson,
                Channels = JsonConvert.DeserializeObject<List<Channel>>(entity.ChannelsJson) ?? new List<Channel>()
            };
        }
    }
}