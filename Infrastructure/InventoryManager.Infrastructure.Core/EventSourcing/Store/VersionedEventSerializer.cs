using InventoryManager.Infrastructure.Core.Serialization;
using System;
using System.IO;

namespace InventoryManager.Infrastructure.Core.EventSourcing.Store
{
	public class VersionedEventSerializer
	{
		private readonly JsonTextSerializer _serializer;

		public VersionedEventSerializer()
		{
			_serializer = new JsonTextSerializer();
		}

		public EventData Serialize(IVersionedEvent e, Type sourceType, string correlationId)
		{
			using (var writer = new StringWriter())
			{
				_serializer.Serialize(writer, e);
				var eventType = e.GetType();
				return new EventData
				{
					Version = e.Version,
					SourceId = e.SourceId.ToString(),
					Payload = writer.ToString(),
					SourceType = sourceType.Name,
					CorrelationId = correlationId,

					AssemblyName = e.GetType().Assembly.FullName,
					Namespace = eventType.Namespace,
					TypeName = eventType.Name,
					FullName = eventType.FullName
				};
			}
		}

		public IVersionedEvent Deserialize(EventData @event)
		{
			using (var reader = new StringReader(@event.Payload))
			{
				return (IVersionedEvent)_serializer.Deserialize(reader);
			}
		}
	}
}