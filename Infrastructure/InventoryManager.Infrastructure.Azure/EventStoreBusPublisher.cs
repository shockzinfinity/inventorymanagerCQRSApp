using InventoryManager.Infrastructure.Core.EventSourcing.PendingEvents;
using InventoryManager.Infrastructure.Core.EventSourcing.Store;
using InventoryManager.Infrastructure.Core.ServiceBus;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryManager.Infrastructure.Azure
{
	public class EventStoreBusPublisher<T> : IEventStoreBusPublisher<T>, IDisposable
	{
		private readonly IServiceBus _sender;
		private readonly IPendingEventsQueue<T> _queue;
		private readonly BlockingCollection<string> _enqueuedKeys;

		public EventStoreBusPublisher(IServiceBus sender, IPendingEventsQueue<T> queue)
		{
			this._sender = sender;
			this._queue = queue;

			_enqueuedKeys = new BlockingCollection<string>();
		}

		public void Start(CancellationToken cancellationToken)
		{
			Task.Factory.StartNew(
				() =>
				{
					try
					{
						foreach (var key in _enqueuedKeys.GetConsumingEnumerable(cancellationToken))
						{
							if (!cancellationToken.IsCancellationRequested)
							{
								ProcessPartition(key);
							}
							else
							{
								EnqueueIfNotExists(key);
								return;
							}
						}
					}
					catch (OperationCanceledException)
					{
						return;
					}
				},
				TaskCreationOptions.LongRunning);

			// Query through all partitions to check for pending events, as there could be
			// stored events that were never published before the system was rebooted.
			Task.Factory.StartNew(
				() =>
				{
					foreach (var partitionKey in _queue.GetSourceIdsWithPendingEvents())
					{
						if (cancellationToken.IsCancellationRequested)
							return;

						EnqueueIfNotExists(partitionKey);
					}
				},
				TaskCreationOptions.LongRunning);
		}

		public void Send(string sourceId, int eventCount)
		{
			if (string.IsNullOrEmpty(sourceId))
				throw new ArgumentNullException(sourceId);

			EnqueueIfNotExists(sourceId);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_enqueuedKeys.Dispose();
			}
		}

		private void EnqueueIfNotExists(string partitionKey)
		{
			if (!_enqueuedKeys.Any(partitionKey.Equals))
			{
				// if the key is not already in the queue, add it. No need to add it if it's already there, as
				// when the partition is processed, it will already try to send all events.
				_enqueuedKeys.Add(partitionKey);
			}
		}

		private void ProcessPartition(string key)
		{
			try
			{
				var events = _queue.GetPendingEventsFor(key);
				foreach (var eventData in events)
				{
					SendAndDeletePending(eventData);
				}

				var hasMoreUnPublishedEvents = _queue.GetPendingEventsFor(key).Any();
				if (hasMoreUnPublishedEvents)
					EnqueueIfNotExists(key);
			}
			catch (Exception ex)
			{
				Trace.TraceError("An error occurred while getting the events pending for publishing for partition {0}:\r\n{1}", key, ex);
				throw;
			}
		}

		private void SendAndDeletePending(EventData eventData)
		{
			var ev = new VersionedEventSerializer().Deserialize(eventData);
			_sender.Publish(ev);
			_queue.DeletePending(eventData);
		}
	}
}