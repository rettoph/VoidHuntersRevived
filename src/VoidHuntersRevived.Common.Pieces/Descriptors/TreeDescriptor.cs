using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Physics.Descriptors;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
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
