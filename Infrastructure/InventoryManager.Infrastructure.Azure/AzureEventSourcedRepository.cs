using InventoryManager.Infrastructure.Core.EventSourcing;
using InventoryManager.Infrastructure.Core.EventSourcing.PendingEvents;
using InventoryManager.Infrastructure.Core.EventSourcing.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InventoryManager.Infrastructure.Azure
{
	public class AzureEventSourcedRepository<T> : IEventSourcedRepository<T> where T : class, IEventSourced
	{
		private static readonly string SourceType = typeof(T).Name;

		private readonly IEventStore<T> _eventStore;
		private readonly IEventStoreBusPublisher<T> _publisher;

		private readonly Func<Guid, IEnumerable<IVersionedEvent>, T> _entityFactory;
		private readonly VersionedEventSerializer _versionedEventSerializer;

		public AzureEventSourcedRepository(IEventStore<T> eventStore, IEventStoreBusPublisher<T> publisher)
		{
			this._eventStore = eventStore;
			this._publisher = publisher;
			_versionedEventSerializer = new VersionedEventSerializer();

			var constructor = typeof(T).GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<IVersionedEvent>) });
			if (constructor == null)
			{
				throw new InvalidCastException(
					"Type T must have a constructor with the following signature: .ctor(Guid, IEnumerable<IVersionedEvent>)");
			}
			_entityFactory = (id, events) => (T)constructor.Invoke(new object[] { id, events });
		}

		public T Find(Guid id)
		{
			var deserialized = this._eventStore.Load(id.ToString(), 0)
				.Select(_versionedEventSerializer.Deserialize);

			if (deserialized.Any())
			{
				return _entityFactory.Invoke(id, deserialized);
			}

			return null;
		}

		public T Get(Guid id)
		{
			var entity = Find(id);
			if (entity == null)
			{
				throw new EntityNotFoundException(id, SourceType);
			}

			return entity;
		}

		public void Save(T eventSourced, string correlationId)
		{
			// TODO: guarantee that only incremental versions of the event are stored
			var events = eventSourced.Events.ToArray();
			var serialized = events.Select(e => _versionedEventSerializer.Serialize(e, typeof(T), correlationId));

			_eventStore.Save(eventSourced.Id.ToString(), serialized);

			_publisher.Send(eventSourced.Id.ToString(), events.Length);
		}
	}
}