using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Serialization.Components;

namespace VoidHuntersRevived.Game.Common.Descriptors
{
    public class ChainDescriptor : TreeDescriptor
    {
        public ChainDescriptor()
        {
            this.ExtendWith(new[]
            {
                new ComponentManager<Tractorable, TractorableComponentSerializer>()
            });
        }
    }
}
