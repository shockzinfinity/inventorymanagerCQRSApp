using InventoryManager.Infrastructure.Core.EventSourcing.Store;
using System.Collections.Generic;

namespace InventoryManager.Infrastructure.Core.EventSourcing.PendingEvents
{
	public interface IPendingEventsQueue<T>
	{
		IEnumerable<string> GetSourceIdsWithPendingEvents();

		IEnumerable<EventData> GetPendingEventsFor(string sourceId);

		void DeletePending(EventData eventData);
	}
}