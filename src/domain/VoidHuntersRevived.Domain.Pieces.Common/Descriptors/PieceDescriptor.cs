using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Teams.Descriptors;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Shared;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    public abstract class PieceDescriptor : TeamMemberEntityDescriptor
    {
        public PieceDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<Id<PieceType>>(),
                new ComponentBuilder<Plug>(in Plug.Default),
                new ComponentBuilder<Coupling>(),
                new ComponentBuilder<Node>(),
                new ComponentBuilder<Rigid>(),
                new ComponentBuilder<ColorScheme>()
            ]);

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
