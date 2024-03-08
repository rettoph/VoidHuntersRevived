using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Pieces;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal sealed class BlueprintConverter : JsonConverter<Blueprint>
    {
        public override Blueprint? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string name = string.Empty;
            IBlueprintPiece head = default!;

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(Blueprint.Name):
                        name = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(Blueprint.Head):
                        head = JsonSerializer.Deserialize<IBlueprintPiece>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            return new Blueprint(name, head);
        }

        public override void Write(Utf8JsonWriter writer, Blueprint value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
