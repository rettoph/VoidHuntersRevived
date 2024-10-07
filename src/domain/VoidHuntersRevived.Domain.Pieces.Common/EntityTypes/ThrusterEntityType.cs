using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(ThrusterEntityType))]
    public class ThrusterEntityType : PieceEntityType
    {
        public ThrusterEntityType(Key<IEntityType> key) : base(key)
        {
            this.WithComponents([
                new Thrustable()
            ]);
        }
    }
}
