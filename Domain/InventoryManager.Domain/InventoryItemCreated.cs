using InventoryManager.Infrastructure.Core.EventSourcing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventoryManager.Domain
{
    public class InventoryItemCreated : VersionedEvent
    {
        public Guid InventoryItemId { get; set; }
        public string Name { get; set; }

        public InventoryItemCreated(Guid id, string name)
        {
            this.InventoryItemId = id;
            this.Name = name;
        }
    }
}
