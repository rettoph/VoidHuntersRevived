using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    internal class FixVector2Converter : JsonConverter<FixVector2>
    {
        public override FixVector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixVector2 result = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(FixVector2.X):
                        result.X = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(FixVector2.Y):
                        result.Y = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, FixVector2 value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
