using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    internal class FixPolarConverter : JsonConverter<FixPolar>
    {
        public override FixPolar Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixPolar result = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(FixPolar.Length):
                        result.Length = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(FixPolar.Radians):
                        result.Radians = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return result;
        }

        public override void Write(Utf8JsonWriter writer, FixPolar value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
