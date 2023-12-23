using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    internal class Fix64Converter : JsonConverter<Fix64>
    {
        public override Fix64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            double doubleValue = reader.GetDouble();
            Fix64 fix64Value = (Fix64)doubleValue;

            return fix64Value;
        }

        public override void Write(Utf8JsonWriter writer, Fix64 value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((double)value);
        }
    }
}
