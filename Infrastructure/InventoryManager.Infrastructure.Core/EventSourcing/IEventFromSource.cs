using InventoryManager.Infrastructure.Core.ServiceBus;
using System;

namespace InventoryManager.Infrastructure.Core.EventSourcing
{
	public interface IEventFromSource : IEvent
	{
		Guid SourceId { get; }
	}
}