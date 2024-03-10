using Guppy.Serialization.Converters;
using Svelto.DataStructures;
using System.Text.Json;

namespace VoidHuntersRevived.Domain.Serialization.Json
{
    public class NativeDynamicArrayCastJsonConverter : GenericTypeDefinitionJsonConverter
    {
        public NativeDynamicArrayCastJsonConverter() : base(typeof(NativeDynamicArrayCast<>), typeof(GenericNativeDynamicArrayCastJsonConverter<>))
        {
        }

        private class GenericNativeDynamicArrayCastJsonConverter<T> : GenericTypeDefinitionJsonConverter.GenericTypeJsonConverter<NativeDynamicArrayCast<T>>
            where T : unmanaged
        {
            protected override NativeDynamicArrayCast<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                T[] array = JsonSerializer.Deserialize<T[]>(ref reader, options) ?? Array.Empty<T>();
                reader.Read();

                return array.ToNativeDynamicArray();
            }

            protected override void Write(Utf8JsonWriter writer, NativeDynamicArrayCast<T> value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }

}
