using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    [AutoLoad]
    public class ThrusterDescriptor : PieceDescriptor
    {
        public ThrusterDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<Thrustable>()
            ]);
        }
    }
}
