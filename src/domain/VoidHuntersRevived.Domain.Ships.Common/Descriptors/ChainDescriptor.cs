using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Ships.Common.Components;
using VoidHuntersRevived.Domain.Ships.Common.Serialization.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    public class ChainDescriptor : TreeDescriptor
    {
        public ChainDescriptor()
        {
            this.WithInstanceComponents(new[]
            {
                new ComponentManager<Tractorable, TractorableComponentSerializer>()
            });
        }
    }
}
