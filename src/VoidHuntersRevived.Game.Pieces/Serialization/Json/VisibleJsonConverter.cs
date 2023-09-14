using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Pieces;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Game.Pieces.Serialization.Json
{
    internal class VisibleJsonConverter : JsonConverter<Visible>
    {
        public override Visible Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            NativeDynamicArrayCast<Shape> fill = default;
            NativeDynamicArrayCast<Shape> trace = default;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Visible.Fill):
                        fill = JsonSerializer.Deserialize<NativeDynamicArrayCast<Shape>>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(Visible.Trace):
                        trace = JsonSerializer.Deserialize<NativeDynamicArrayCast<Shape>>(ref reader, options);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Visible()
            {
                Fill = fill,
                Trace = trace
            };
        }

        public override void Write(Utf8JsonWriter writer, Visible value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
