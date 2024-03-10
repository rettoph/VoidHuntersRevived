using Svelto.DataStructures;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.FixedPoint;
using VoidHuntersRevived.Domain.Physics.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class RigidJsonConverter : JsonConverter<Rigid>
    {
        public override Rigid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixVector2 centeroid = default;
            NativeDynamicArrayCast<Polygon> shapes = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Rigid.Centeroid):
                        centeroid = JsonSerializer.Deserialize<FixVector2>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(Rigid.Shapes):
                        shapes = JsonSerializer.Deserialize<NativeDynamicArrayCast<Polygon>>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Rigid()
            {
                Centeroid = centeroid,
                Shapes = shapes
            };
        }

        public override void Write(Utf8JsonWriter writer, Rigid value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
