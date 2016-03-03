using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Formatting = System.Xml.Formatting;

namespace InventoryManager.Infrastructure.Core.Serialization
{
    public class JsonTextSerializer
    {
        private readonly JsonSerializer _serializer;

        public JsonTextSerializer()
            : this(JsonSerializer.Create(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
            }))
        {
        }

        public JsonTextSerializer(JsonSerializer serializer)
        {
            this._serializer = serializer;
        }

        public void Serialize(TextWriter writer, object graph)
        {
            var jsonWriter = new JsonTextWriter(writer);
            _serializer.Serialize(jsonWriter, graph);

            writer.Flush();
        }

        public object Deserialize(TextReader reader)
        {
            var jsonReader = new JsonTextReader(reader);

            try
            {
                return this._serializer.Deserialize(jsonReader);
            }
            catch (JsonSerializationException e)
            {
                throw new SerializationException(e.Message, e);
            }
        }
    }
}
