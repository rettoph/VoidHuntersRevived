using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class RigidJsonConverter(IResourceService resources) : JsonConverter<Rigid>
    {
        private readonly IResourceService _resources = resources;

        public override Rigid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            ResourceValue<IBodyTemplate> template = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Rigid.Template):
                        template = JsonSerializer.Deserialize<ResourceValue<IBodyTemplate>>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Rigid(template);
        }

        public override void Write(Utf8JsonWriter writer, Rigid value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
