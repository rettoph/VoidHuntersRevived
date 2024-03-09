using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public class ThrusterDescriptor : PieceDescriptor
    {
        public ThrusterDescriptor() : base(Resources.Colors.ThrusterPrimaryColor, Resources.Colors.ThrusterSecondaryColor, -1)
        {
            this.WithInstanceComponents(new[]
            {
                new ComponentManager<Thrustable, ThrustableComponentSerializer>()
            });
        }
    }
}
