using Svelto.DataStructures;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class SocketsJsonConverter : JsonConverter<Sockets>
    {
        public override Sockets Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            NativeDynamicArrayCast<Socket> items = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Sockets.Items):
                        items = JsonSerializer.Deserialize<NativeDynamicArrayCast<Socket>>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Sockets()
            {
                Items = items
            };
        }

        public override void Write(Utf8JsonWriter writer, Sockets value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
