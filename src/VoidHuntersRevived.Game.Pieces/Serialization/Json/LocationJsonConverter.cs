using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Game.Pieces.Serialization.Json
{
    internal class LocationJsonConverter : JsonConverter<Location>
    {
        public override Location Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            FixVector2 position = default;
            Fix64 rotation = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Location.Position):
                        position = JsonSerializer.Deserialize<FixVector2>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(Location.Rotation):
                        rotation = JsonSerializer.Deserialize<Fix64>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Location(position, rotation);
        }

        public override void Write(Utf8JsonWriter writer, Location value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
