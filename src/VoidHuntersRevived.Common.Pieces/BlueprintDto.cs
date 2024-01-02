using System.Text.Json.Serialization;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Core.Utilities;
using VoidHuntersRevived.Common.Entities;

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
