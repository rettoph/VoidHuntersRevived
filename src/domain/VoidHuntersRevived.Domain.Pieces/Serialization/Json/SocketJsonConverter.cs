using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class SocketJsonConverter : JsonConverter<Socket>
    {
        public override Socket Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixTransform2D localTransform = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Socket.NodeTransform):
                        localTransform = JsonSerializer.Deserialize<FixTransform2D>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Socket(localTransform);
        }

        public override void Write(Utf8JsonWriter writer, Socket value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}