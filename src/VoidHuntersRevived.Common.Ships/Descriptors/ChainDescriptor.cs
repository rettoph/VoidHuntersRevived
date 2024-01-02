using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Descriptors;
using VoidHuntersRevived.Common.Ships.Components;
using VoidHuntersRevived.Common.Ships.Serialization.Components;

namespace VoidHuntersRevived.Common.Ships.Descriptors
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
