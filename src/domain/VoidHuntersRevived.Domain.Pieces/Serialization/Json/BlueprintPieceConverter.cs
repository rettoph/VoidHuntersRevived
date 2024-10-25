using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal sealed class BlueprintPieceConverter : JsonConverter<IBlueprintPiece>
    {
        public override IBlueprintPiece? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Key<IEntityTemplate>? pieceEntityTemplateKey = null;
            IBlueprintPiece[][] children = Array.Empty<IBlueprintPiece[]>();

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(IBlueprintPiece.PieceTemplateKey):
                        pieceEntityTemplateKey = JsonSerializer.Deserialize<Key<IEntityTemplate>>(ref reader, options);
                        reader.Read();
                        break;
                    case nameof(IBlueprintPiece.Children):
                        children = JsonSerializer.Deserialize<IBlueprintPiece[][]>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            if (pieceEntityTemplateKey is null)
            {
                throw new ArgumentException();
            }

            for (int i = 0; i < children.Length; i++)
            {
                children[i] ??= Array.Empty<IBlueprintPiece>();
            }

            return new BlueprintPiece(pieceEntityTemplateKey.Value, children);
        }

        public override void Write(Utf8JsonWriter writer, IBlueprintPiece value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
