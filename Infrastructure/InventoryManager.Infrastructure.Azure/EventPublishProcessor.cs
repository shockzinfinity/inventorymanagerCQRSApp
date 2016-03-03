using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InventoryManager.Infrastructure.Core;
using InventoryManager.Infrastructure.Core.EventSourcing.PendingEvents;

namespace InventoryManager.Infrastructure.Azure
{
    public class EventPublishProcessor<T> : IProcessor
    {
        private readonly IEventStoreBusPublisher<T> _publisher;
        private readonly CancellationTokenSource _tokenSource;

        public EventPublishProcessor(IEventStoreBusPublisher<T> publisher)
        {
            _publisher = publisher;
            _tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            _publisher.Start(_tokenSource.Token);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }
    }
}
