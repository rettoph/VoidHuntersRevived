using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(ThrusterEntityType))]
    public class ThrusterEntityType : PieceEntityType
    {
        public ThrusterEntityType(IKey<ThrusterEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
            this.WithInstanceEntityComponents([
                new Thrustable()
            ]);
        }
    }
}
