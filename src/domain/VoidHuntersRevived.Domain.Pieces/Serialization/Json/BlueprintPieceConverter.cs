using System.Text.Json;
using System.Text.Json.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Json
{
    internal sealed class BlueprintPieceConverter : JsonConverter<IBlueprintPiece>
    {
        private readonly Lazy<IEntityTypeService> _entityTypes;

        public BlueprintPieceConverter(Lazy<IEntityTypeService> entityTypes)
        {
            _entityTypes = entityTypes;
        }

        public override IBlueprintPiece? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string pieceTypeKey = string.Empty;
            IBlueprintPiece[][] children = Array.Empty<IBlueprintPiece[]>();

            reader.CheckToken(JsonTokenType.StartObject, true);
            reader.Read();

            while (reader.ReadPropertyName(out string? propertyName))
            {
                switch (propertyName)
                {
                    case nameof(IBlueprintPiece.PieceType):
                        pieceTypeKey = JsonSerializer.Deserialize<string>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                    case nameof(IBlueprintPiece.Children):
                        children = JsonSerializer.Deserialize<IBlueprintPiece[][]>(ref reader, options) ?? throw new NotImplementedException();
                        reader.Read();
                        break;
                }
            }

            reader.CheckToken(JsonTokenType.EndObject, true);

            for (int i = 0; i < children.Length; i++)
            {
                children[i] ??= Array.Empty<IBlueprintPiece>();
            }

            return new BlueprintPiece(pieceTypeKey, children, _entityTypes);
        }

        public override void Write(Utf8JsonWriter writer, IBlueprintPiece value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
