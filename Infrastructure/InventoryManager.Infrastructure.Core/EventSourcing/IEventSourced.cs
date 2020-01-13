using System;
using System.Collections.Generic;

namespace InventoryManager.Infrastructure.Core.EventSourcing
{
	public interface IEventSourced
	{
		Guid Id { get; }
		int Version { get; }
		IEnumerable<IVersionedEvent> Events { get; }
	}
}