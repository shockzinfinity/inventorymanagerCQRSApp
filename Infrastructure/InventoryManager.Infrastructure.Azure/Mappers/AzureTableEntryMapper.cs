using System;
using InventoryManager.Infrastructure.Core.EventSourcing;
using InventoryManager.Infrastructure.Core.EventSourcing.Store;

namespace InventoryManager.Infrastructure.Azure.Mappers
{
    public static class AzureTableEntryMapper
    {

        public static EventTableServiceEntity ToAzureTableEntry(this EventData eventData, DateTime creationDate)
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
            return version.ToString("D10");
        }

        private static int GetVersionFromRowKey(string rowKey)
        {   
            return int.Parse(rowKey);
        }

    }
}
