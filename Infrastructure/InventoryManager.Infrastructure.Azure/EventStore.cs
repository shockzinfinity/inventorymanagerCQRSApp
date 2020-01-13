using InventoryManager.Infrastructure.Azure.Mappers;
using InventoryManager.Infrastructure.Core;
using InventoryManager.Infrastructure.Core.EventSourcing.PendingEvents;
using InventoryManager.Infrastructure.Core.EventSourcing.Store;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Net;

namespace InventoryManager.Infrastructure.Azure
{
	public class EventStore<T> : IEventStore<T>, IPendingEventsQueue<T>
	{
		private readonly string _tableName;
		private readonly CloudTableClient _tableClient;

		public EventStore(CloudStorageAccount account, string tableName)
		{
			if (account == null) throw new ArgumentNullException("account");
			if (tableName == null) throw new ArgumentNullException("tableName");
			if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("tableName");

			_tableName = tableName;
			_tableClient = account.CreateCloudTableClient();
			_tableClient.DefaultRequestOptions.RetryPolicy = new NoRetry();
			var table = _tableClient.GetTableReference(tableName);
			table.CreateIfNotExists();
		}

		public IEnumerable<EventData> Load(string sourceId, int fromVersion)
		{
			var minRowKey = AzureTableEntryMapper.GetRowKeyFromVersion(fromVersion);
			var query = GetEntitiesQuery(sourceId, minRowKey, RowKeyConstants.RowKeyUpperLimit);
			var all = query.Execute();
			return all.Select(x => AzureTableEntryMapper.ToEventData(x));
		}

		public void Save(string sourceId, IEnumerable<EventData> events)
		{
			var table = _tableClient.GetTableReference(_tableName);
			var tableBatchOperation = new TableBatchOperation();
			foreach (var eventData in events)
			{
				if (eventData.SourceId != sourceId)
					throw new Exception("Events from different aggregate instances found during EventStore save. Events from only single Aggregate instance can be saved.");

				var creationDate = DateTime.UtcNow;

				tableBatchOperation.Insert(eventData.ToAzureTableEntry(creationDate));

				// Add a duplicate of this event to the Unpublished "queue"
				tableBatchOperation.Insert(eventData.ToUnpublishedAzureTableEntry(creationDate));
			}

			try
			{
				table.ExecuteBatch(tableBatchOperation);
			}
			catch (DataServiceRequestException ex)
			{
				var inner = ex.InnerException as DataServiceClientException;
				if (inner != null && inner.StatusCode == (int)HttpStatusCode.Conflict)
				{
					throw new ConcurrencyException();
				}

				throw;
			}
		}

		public IEnumerable<EventData> GetPendingEventsFor(string sourceId)
		{
			var query = GetEntitiesQuery(sourceId, RowKeyConstants.UnpublishedRowKeyPrefix,
												   RowKeyConstants.UnpublishedRowKeyPrefixUpperLimit);
			var table = _tableClient.GetTableReference(_tableName);
			return table.ExecuteQuery(query).Select(x => UnpublishedAzureTableEntryMapper.ToEventData(x));
		}

		public void DeletePending(EventData eventData)
		{
			var table = _tableClient.GetTableReference(_tableName);
			var partitionKey = eventData.SourceId;
			var rowKey = UnpublishedAzureTableEntryMapper.GetRowKeyFromVersion(eventData.Version);
			var retrieveOperation = TableOperation.Retrieve<EventTableServiceEntity>(partitionKey,
																					 rowKey);
			var retrievedResult = table.Execute(retrieveOperation);
			var fetchedItem = retrievedResult.Result as EventTableServiceEntity;

			if (fetchedItem == null)
				return; //already deleted

			var deleteOperation = TableOperation.Delete(fetchedItem);
			table.Execute(deleteOperation);
		}

		public IEnumerable<string> GetSourceIdsWithPendingEvents()
		{
			var partitionKeyQuery = (new TableQuery<EventTableServiceEntity>()).Where(TableQuery.CombineFilters(
			   TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, RowKeyConstants.UnpublishedRowKeyPrefix),
			   TableOperators.And,
			   TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, RowKeyConstants.UnpublishedRowKeyPrefixUpperLimit)));

			var table = _tableClient.GetTableReference(_tableName);
			return table.ExecuteQuery(partitionKeyQuery).Select(x => x.PartitionKey);
		}

		private TableQuery<EventTableServiceEntity> GetEntitiesQuery(string partitionKey, string minRowKey, string maxRowKey)
		{
			var rowKeyRangeFilter = TableQuery.CombineFilters(
				TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThanOrEqual, minRowKey),
				TableOperators.And,
				TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThanOrEqual, maxRowKey));

			var entitiesQuery = new TableQuery<EventTableServiceEntity>().Where(
				TableQuery.CombineFilters(
					TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey),
					TableOperators.And,
					rowKeyRangeFilter));

			return entitiesQuery;
		}
	}
}