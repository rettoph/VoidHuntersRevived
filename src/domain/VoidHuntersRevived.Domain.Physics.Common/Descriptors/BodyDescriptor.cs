using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Physics.Serialization.Components;

namespace VoidHuntersRevived.Common.Physics.Descriptors
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
