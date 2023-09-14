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
    internal class Fix64Converter : JsonConverter<Fix64>
    {
        public override Fix64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Fix64 value = (Fix64)reader.GetDecimal();

            return value;
        }

        public override void Write(Utf8JsonWriter writer, Fix64 value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
