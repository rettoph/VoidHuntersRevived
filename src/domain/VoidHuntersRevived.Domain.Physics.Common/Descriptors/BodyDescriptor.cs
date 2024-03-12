using Svelto.ECS;
using VoidHuntersRevived.Common.Teams.Descriptors;
using VoidHuntersRevived.Domain.Physics.Common.Components;

namespace VoidHuntersRevived.Domain.Physics.Common.Descriptors
{
    public abstract class BodyDescriptor : TeamMemberEntityDescriptor
    {
        public BodyDescriptor() : base()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<Collision>(),
                new ComponentBuilder<Location>(new Location()),
                new ComponentBuilder<Enabled>(),
                new ComponentBuilder<Awake>(new Awake(true)),
            ]);
        }
    }
}
