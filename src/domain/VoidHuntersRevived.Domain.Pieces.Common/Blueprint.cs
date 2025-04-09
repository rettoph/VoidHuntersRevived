using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Utilities;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Extensions;

namespace VoidHuntersRevived.Domain.Pieces.Common
{
    public sealed class Blueprint(string name, IBlueprintPiece head) : IEntityAsset<Blueprint>
    {
        private Id<Blueprint>? _id;

        public Id<Blueprint> Id => this._id ??= HashBuilder<Blueprint, VhId, VhId>.Instance.CalculateId(VhId.HashString(this.Name), this.Head.CalculateHash());
        public readonly string Name = name;
        public readonly IBlueprintPiece Head = head;
    }
}