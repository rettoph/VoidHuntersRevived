using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    public abstract class TreeDescriptor : BodyDescriptor
    {
        public TreeDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<Tree>(),
                new ComponentBuilder<HasMany<Node, Tree>>()
            ]);
        }
    }
}
