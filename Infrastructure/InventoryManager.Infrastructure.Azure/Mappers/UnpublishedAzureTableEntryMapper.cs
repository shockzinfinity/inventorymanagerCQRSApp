using System;
using InventoryManager.Infrastructure.Core.EventSourcing;
using InventoryManager.Infrastructure.Core.EventSourcing.Store;

namespace InventoryManager.Infrastructure.Azure.Mappers
{
    public static class UnpublishedAzureTableEntryMapper
    {
        public static EventTableServiceEntity ToUnpublishedAzureTableEntry(this EventData eventData, DateTime creationDate)
        {
            return new EventTableServiceEntity
            {
                PartitionKey = eventData.SourceId,
                RowKey = GetRowKeyFromVersion(eventData.Version),

                SourceId = eventData.SourceId,
                SourceType = eventData.SourceType,
                Payload = eventData.Payload,
                CorrelationId = eventData.CorrelationId,

                AssemblyName = eventData.AssemblyName,
                Namespace = eventData.Namespace,
                FullName = eventData.FullName,
                TypeName = eventData.TypeName,

                CreationDate = creationDate.ToString("o")
            };
        }

        public static EventData ToEventData(EventTableServiceEntity azureTableEntity)
        {
            return new EventData()
            {
                Version = GetVersionFromRowKey(azureTableEntity.RowKey),

                SourceId = azureTableEntity.SourceId,
                SourceType = azureTableEntity.SourceType,
                Payload = azureTableEntity.Payload,
                CorrelationId = azureTableEntity.CorrelationId,

                AssemblyName = azureTableEntity.AssemblyName,
                Namespace = azureTableEntity.Namespace,
                FullName = azureTableEntity.FullName,
                TypeName = azureTableEntity.TypeName,
            };
        }

        public static string GetRowKeyFromVersion(int version)
        {
            return RowKeyConstants.UnpublishedRowKeyPrefix + version.ToString("D10");
        }

        public static int GetVersionFromRowKey(string rowKeyForUnpublishedEvent)
        {
            var formattedVersion = rowKeyForUnpublishedEvent.Remove(0, RowKeyConstants.UnpublishedRowKeyPrefix.Length);
            return int.Parse(formattedVersion);
        }


    }
}
