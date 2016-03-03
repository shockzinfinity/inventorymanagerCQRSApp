using System;
using InventoryManager.Infrastructure.Core.ServiceBus;

namespace InventoryManager.Infrastructure.Core.EventSourcing
{
    public interface IEventFromSource : IEvent
    {
        Guid SourceId { get; }
    }
}
