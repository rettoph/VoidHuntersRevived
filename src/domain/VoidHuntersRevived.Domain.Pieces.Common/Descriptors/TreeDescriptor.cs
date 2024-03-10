using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    public abstract class TreeDescriptor : BodyDescriptor
    {
        public TreeDescriptor()
        {
            this.WithInstanceComponents(new ComponentManager[]
            {
                new ComponentManager<Tree, TreeComponentSerializer>(),
                new ComponentManager<Id<Blueprint>, BlueprintIdComponentSerializer>()
            });
        }
    }
}
