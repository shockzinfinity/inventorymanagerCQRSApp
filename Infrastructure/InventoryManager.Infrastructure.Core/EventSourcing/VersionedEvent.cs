using System;

namespace InventoryManager.Infrastructure.Core.EventSourcing
{
	public abstract class VersionedEvent : IVersionedEvent
	{
		public Guid SourceId { get; set; }
		public int Version { get; set; }
	}
}