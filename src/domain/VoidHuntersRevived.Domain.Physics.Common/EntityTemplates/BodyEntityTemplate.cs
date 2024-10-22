using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Teams.Common.EntityTemplates;

namespace VoidHuntersRevived.Domain.Physics.Common.EntityTemplates
{
    [PolymorphicJsonType<IEntityTemplate>(nameof(BodyEntityTemplate))]
    public abstract class BodyEntityTemplate : TeamMemberEntityTemplate
    {
        public BodyEntityTemplate(Key<IEntityTemplate> key) : base(key)
        {
            this.WithComponents([
                new Location(),
                new Enabled(),
                new Awake(true)
            ]);

            this.RequireComponents([
                typeof(Collision)
            ]);
        }
    }
}
