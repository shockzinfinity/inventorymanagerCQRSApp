using System;
using System.Diagnostics;
using InventoryManager.Domain;
using InventoryManager.Infrastructure.Core.EventSourcing;
using InventoryManager.Infrastructure.Core.ServiceBus;

namespace InventoryManager.Application.InventoryItems
{
    public class InventoryItemAppService : IHandle<CreateInventoryItem>
    {
        private readonly IEventSourcedRepository<InventoryItem> _inventoryItemRepository;

        public InventoryItemAppService(IEventSourcedRepository<InventoryItem> inventoryItemRepository)
        {
            _inventoryItemRepository = inventoryItemRepository;
        }

        public void Handle(CreateInventoryItem command)
        {
            Trace.TraceInformation("CreateInventory Item message received: For {0} - {1} - {2}", command.Id, command.Name, DateTime.Now);
            
            var inventoryItem = new InventoryItem(command.Id, command.Name);
            _inventoryItemRepository.Save(inventoryItem, command.Id.ToString());
        }
    }
}
