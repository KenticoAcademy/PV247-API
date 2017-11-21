using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messaging.Data
{
    internal class AzureTableHelper
    {
        public static async Task<IEnumerable<T>> GetSegmentedResult<T>(CloudTable table, TableQuery<T> query)
            where T : ITableEntity, new()
        {
            TableContinuationToken continuationToken = null;
            var items = new List<T>();
            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                if (segment != null)
                {
                    continuationToken = segment.ContinuationToken;
                    items.AddRange(segment.Results);
                }
            } while (continuationToken != null);

            return items;
        }

        public static TableQuery<T> GetRowKeyPrefixQuery<T>(string partitionKey, string rowKeyPrefix)
            where T : ITableEntity, new()
        {
            var query = new TableQuery<T>();

            var length = rowKeyPrefix.Length - 1;
            var lastChar = rowKeyPrefix[length];

            var nextLastChar = (char)(lastChar + 1);

            var startsWithEndPattern = rowKeyPrefix.Substring(0, length) + nextLastChar;

            var prefixCondition = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition("RowKey",
                    QueryComparisons.GreaterThanOrEqual,
                    rowKeyPrefix),
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
    }
}