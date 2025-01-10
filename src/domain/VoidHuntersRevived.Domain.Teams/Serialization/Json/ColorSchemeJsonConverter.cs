using System.Text.Json;
using System.Text.Json.Serialization;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Teams.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal class ColorSchemeJsonConverter(IResourceService resources) : JsonConverter<ColorScheme>
    {
        private readonly IResourceService _resources = resources;

        public override ColorScheme Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ResourceKey<Color> primary = default!;
            ResourceKey<Color> secondary = default!;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(ColorScheme.Primary):
                        string primaryKey = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        primary = ResourceKey<Color>.Get(primaryKey);
                        reader.Read();
                        break;
                    case nameof(ColorScheme.Secondary):
                        string secondaryKey = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        secondary = ResourceKey<Color>.Get(secondaryKey);
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new ColorScheme(this._resources.Get(primary), this._resources.Get(secondary));
        }

        public override void Write(Utf8JsonWriter writer, ColorScheme value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}