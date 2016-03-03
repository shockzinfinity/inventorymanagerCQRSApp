using System;
using System.Collections.Generic;
using InventoryManager.Infrastructure.Core.EventSourcing;

namespace InventoryManager.Domain
{
    public class InventoryItem : EventSourced
    {
        private string _name = string.Empty;//Rename functionality will use this field

        protected InventoryItem(Guid id)
            : base(id)
        {
            Handles<InventoryItemCreated>(OnInventoryItemCreated);
        }

        public InventoryItem(Guid id, IEnumerable<IVersionedEvent> events):this(id)
        {
            base.LoadFrom(events);
        }

        private void OnInventoryItemCreated(InventoryItemCreated e)
        {
            this._name = e.Name;
        }

        public InventoryItem(Guid id, string name):this(id)
        {
            Update(new InventoryItemCreated(id, name));
        }
    }
}