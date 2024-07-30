using Guppy.Core.Resources.Common.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class zIndexJsonConverter : JsonConverter<zIndex>
    {
        private readonly IResourceService _resources;

        public zIndexJsonConverter(IResourceService resources)
        {
            _resources = resources;
        }

        public override zIndex Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            short value = 0;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(zIndex.Value):
                        value = reader.ReadInt16();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new zIndex(value);
        }

        public override void Write(Utf8JsonWriter writer, zIndex value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
