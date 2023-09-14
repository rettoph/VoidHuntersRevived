using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    public class NativeDynamicArrayCastJsonConverter<T> : JsonConverter<NativeDynamicArrayCast<T>>
        where T : struct
    {
        public override NativeDynamicArrayCast<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            T[] array = JsonSerializer.Deserialize<T[]>(ref reader, options) ?? Array.Empty<T>();
            reader.Read();

            return array.ToNativeDynamicArray();
        }

        public override void Write(Utf8JsonWriter writer, NativeDynamicArrayCast<T> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
