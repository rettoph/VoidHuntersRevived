using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.Serialization.Components;

namespace VoidHuntersRevived.Domain.Physics.Common.Descriptors
{
    public abstract class BodyDescriptor : VoidHuntersEntityDescriptor
    {
        public BodyDescriptor() : base()
        {
            this.WithInstanceComponents(new ComponentManager[]
            {
                new ComponentManager<Collision, CollisionComponentSerializer>(),
                new ComponentManager<Location, LocationComponentSerializer>(new Location()),
                new ComponentManager<Enabled, EnabledComponentSerializer>(),
                new ComponentManager<Awake, AwakeComponentSerializer>(new Awake(true)),
            });
        }
    }
}
