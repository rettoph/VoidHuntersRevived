using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Pieces
{
    public class BlueprintPieceDto
    {
        [JsonIgnore]
        public VhId Hash => HashBuilder<BlueprintPieceDto, VhId>.Instance.Calculate(this.Key);

        public string Key { get; set; } = string.Empty;

        public BlueprintPieceDto[]?[]? Children { get; set; } = Array.Empty<BlueprintPieceDto[]>();
    }
}
