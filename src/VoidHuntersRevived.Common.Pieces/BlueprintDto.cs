using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Utilities;

namespace VoidHuntersRevived.Common.Pieces
{
    public class BlueprintDto
    {
        [JsonIgnore]
        public Id<IBlueprint> Id => new Id<IBlueprint>(HashBuilder<BlueprintDto, VhId, VhId>.Instance.Calculate(this.Name, this.Head.Hash));

        public string Name { get; set; } = string.Empty;
        public BlueprintPieceDto Head { get; set; } = default!;
    }
}
