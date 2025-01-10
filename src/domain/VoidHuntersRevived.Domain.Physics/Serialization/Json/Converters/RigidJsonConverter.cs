using System.Text.Json;
using System.Text.Json.Serialization;
using Guppy.Core.Resources.Common;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Physics.Serialization.Json
{
    internal class RigidJsonConverter : JsonConverter<Rigid>
    {
        public override Rigid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Resource<IBodyTemplate> template = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Rigid.Template):
                        template = JsonSerializer.Deserialize<Resource<IBodyTemplate>>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Rigid(template);
        }

        public override void Write(Utf8JsonWriter writer, Rigid value, JsonSerializerOptions options) => throw new NotImplementedException();
    }
}