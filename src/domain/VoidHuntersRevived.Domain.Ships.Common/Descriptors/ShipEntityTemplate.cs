using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.EntityTemplates;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    [PolymorphicJsonType<IEntityTemplate>(nameof(ShipEntityTemplate))]
    public abstract class ShipEntityTemplate : TreeEntityTemplate
    {
        public ShipEntityTemplate(Key<IEntityTemplate> id) : base(id)
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
