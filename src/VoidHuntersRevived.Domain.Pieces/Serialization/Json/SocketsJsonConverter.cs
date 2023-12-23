using Guppy.Serialization.Converters;
using Svelto.DataStructures;
using System.Text.Json;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class SocketsJsonConverter : GenericTypeDefinitionJsonConverter
    {
        public SocketsJsonConverter() : base(typeof(Sockets<>), typeof(GenericSocketsJsonConverter<>))
        {
        }

        private class GenericSocketsJsonConverter<T> : GenericTypeDefinitionJsonConverter.GenericTypeJsonConverter<Sockets<T>>
            where T : unmanaged
        {
            protected override Sockets<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                NativeDynamicArrayCast<T> items = default;

                reader.CheckToken(JsonTokenType.StartObject, true);
                reader.Read();

                while (reader.ReadPropertyName(out string? propertyName))
                {
                    switch (propertyName)
                    {
                        case nameof(Sockets<T>.Items):
                            items = JsonSerializer.Deserialize<NativeDynamicArrayCast<T>>(ref reader, options);
                            reader.Read();
                            break;
                    }
                }

                reader.CheckToken(JsonTokenType.EndObject, true);

                return new Sockets<T>()
                {
                    Items = items
                };
            }

            protected override void Write(Utf8JsonWriter writer, Sockets<T> value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
