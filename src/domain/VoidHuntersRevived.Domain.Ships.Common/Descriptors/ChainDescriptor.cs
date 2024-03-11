using Svelto.ECS;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    public class ChainDescriptor : TreeDescriptor
    {
        public ChainDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<Tractorable>()
            ]);
        }
    }
}
