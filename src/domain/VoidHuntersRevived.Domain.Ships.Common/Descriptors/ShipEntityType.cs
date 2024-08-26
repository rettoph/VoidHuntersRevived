using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.EntityTypes;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    [PolymorphicJsonType<IEntityType>(nameof(ShipEntityType))]
    public abstract class ShipEntityType : TreeEntityType
    {
        public ShipEntityType(IKey<IEntityType> id, IKey<IEntityType>[] include) : base(id, include)
        {
            this.WithComponents([
                new PhysicsBubble() {
                    Enabled = false,
                    Radius = default
                },
                new Helm(),
                new Tactical(),
                new TractorBeamEmitter()
                {
                    Active = false
                }
            ]);
        }
    }
}
