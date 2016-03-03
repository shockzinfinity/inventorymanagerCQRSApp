using System.Threading;

namespace InventoryManager.Infrastructure.Core.EventSourcing.PendingEvents
{
    public interface IEventStoreBusPublisher<T>
    {
        void Start(CancellationToken cancellationToken);
        void Send(string sourceId, int eventCount);
    }
}