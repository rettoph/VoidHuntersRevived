using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Shared;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    public class ThrusterDescriptor : PieceDescriptor
    {
        public ThrusterDescriptor()
        {
            this.WithInstanceComponents(new[]
            {
                new ComponentManager<Thrustable, ThrustableComponentSerializer>()
            });
        }

        protected override ResourceColorScheme GetDefaultColorScheme()
        {
            return new ResourceColorScheme(Resources.Colors.ThrusterPrimaryColor, Resources.Colors.ThrusterSecondaryColor);
        }

        protected override zIndex GetZIndex()
        {
            return new zIndex(-1);
        }
    }
}
