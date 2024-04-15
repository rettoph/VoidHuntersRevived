using Guppy.Core.Resources;
using Microsoft.Xna.Framework;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class ColorSchemeJsonConverter : JsonConverter<ColorScheme>
    {
        public override ColorScheme Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Resource<Color> primary = default!;
            Resource<Color> secondary = default!;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(ColorScheme.Primary):
                        string primaryKey = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        primary = Resource<Color>.Get(primaryKey);
                        reader.Read();
                        break;
                    case nameof(ColorScheme.Secondary):
                        string secondaryKey = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        secondary = Resource<Color>.Get(secondaryKey);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new ColorScheme(primary, secondary);
        }

        public override void Write(Utf8JsonWriter writer, ColorScheme value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
