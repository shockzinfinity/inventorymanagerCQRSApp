using System.Collections.Generic;

namespace InventoryManager.Infrastructure.Core.EventSourcing.Store
{
	public interface IEventStore<T>
	{
		IEnumerable<EventData> Load(string sourceId, int fromVersion);

		void Save(string sourceId, IEnumerable<EventData> events);
	}
}