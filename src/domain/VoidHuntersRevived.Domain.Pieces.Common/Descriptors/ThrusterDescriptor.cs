using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Pieces.Components.Instance;
using VoidHuntersRevived.Common.Pieces.Components.Shared;
using VoidHuntersRevived.Common.Pieces.Components.Static;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
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
