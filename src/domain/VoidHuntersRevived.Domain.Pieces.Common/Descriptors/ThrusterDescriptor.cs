using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;

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

        protected override zIndex GetZIndex()
        {
            return new zIndex(-1);
        }
    }
}
