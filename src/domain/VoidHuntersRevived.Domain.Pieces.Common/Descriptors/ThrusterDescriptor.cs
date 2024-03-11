using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Shared;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    public class ThrusterDescriptor : PieceDescriptor
    {
        public ThrusterDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<Thrustable>()
            ]);
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
