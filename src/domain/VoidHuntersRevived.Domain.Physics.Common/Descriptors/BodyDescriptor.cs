using Svelto.ECS;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.Descriptors;

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
