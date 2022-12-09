using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Library.Serialization.Json.Converters
{
    internal sealed class TickJsonConverter : JsonConverter<Tick>
    {
        private enum Properties
        {
            Id,
            Data
        }

        public override Tick? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int id = default;
            List<ITickData> datum = new List<ITickData>();

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();
            
            while(reader.ReadProperty<Properties>(out Properties property))
            {
                switch (property)
                {
                    case Properties.Id:
                        id = reader.ReadInt32();
                        break;
                    case Properties.Data:
                        datum.AddRange(
                            collection : JsonSerializer.Deserialize<IEnumerable<ITickData>>(ref reader, options) ?? Enumerable.Empty<ITickData>()
                        );

                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Tick(id, datum);
        }

        public override void Write(Utf8JsonWriter writer, Tick value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
