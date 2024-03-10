using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Shared;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    public abstract class PieceDescriptor : VoidHuntersEntityDescriptor
    {
        public PieceDescriptor()
        {
            this.WithInstanceComponents(new ComponentManager[]
            {
                new ComponentManager<Id<PieceType>, PieceTypeIdComponentSerializer>(),
                new ComponentManager<Plug, PlugComponentSerializer>(in Plug.Default),
                new ComponentManager<Coupling, CouplingComponentSerializer>(),
                new ComponentManager<Node, NodeComponentSerializer>(),
                new ComponentManager<Rigid, RigidComponentSerializer>(),
                new ComponentManager<ColorScheme, ColorPaletteComponentSerializer>()
            });

            this.WithStaticComponents(new IComponentBuilder[]
            {
                new ComponentBuilder<Visible>(),
                new ComponentBuilder<zIndex>(this.GetZIndex()),
                new ComponentBuilder<ResourceColorScheme>(this.GetDefaultColorScheme())
            });
        }

        protected abstract zIndex GetZIndex();

        protected abstract ResourceColorScheme GetDefaultColorScheme();
    }
}
