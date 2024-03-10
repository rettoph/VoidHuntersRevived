using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Pieces.Components.Instance;
using VoidHuntersRevived.Common.Pieces.Components.Shared;
using VoidHuntersRevived.Common.Pieces.Components.Static;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
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
