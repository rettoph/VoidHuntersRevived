using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.EntityTemplates;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.EntityTemplates
{
    [PolymorphicJsonType<IEntityTemplate>(nameof(TreeEntityTemplate))]
    public abstract class TreeEntityTemplate : BodyEntityTemplate
    {
        public TreeEntityTemplate(Key<IEntityTemplate> key) : base(key)
        {
            this.WithComponents([
                new Tree(),
                new HasMany<Node, Tree>()
            ]);
        }
    }
}
