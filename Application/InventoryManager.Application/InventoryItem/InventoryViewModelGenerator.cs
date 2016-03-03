using System.Diagnostics;
using InventoryManager.Domain;
using InventoryManager.Infrastructure.Core.ServiceBus;

namespace InventoryManager.Application.InventoryItems
{
    public class InventoryViewModelGenerator : IHandle<InventoryItemCreated>
    {
        public void Handle(InventoryItemCreated message)
        {
            Trace.TraceInformation("InventoryViewModelGenerator Processing the message InventoryItemCreated - {0} - {1}", message.InventoryItemId, message.Name);
        }
    }
}