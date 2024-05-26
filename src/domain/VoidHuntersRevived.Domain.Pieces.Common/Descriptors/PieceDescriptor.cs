using Svelto.ECS;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Teams.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Pieces.Common.Descriptors
{
    public abstract class PieceDescriptor : TeamMemberEntityDescriptor
    {
        public PieceDescriptor()
        {
            this.WithInstanceComponents([
                new ComponentBuilder<Plug>(in Plug.Default),
                new ComponentBuilder<Coupling>(),
                new ComponentBuilder<Node>(),
                new ComponentBuilder<Rigid>()
            ]);

            this.WithTypeComponents([
                new ComponentBuilder<Visible>(),
                new ComponentBuilder<zIndex>(this.GetZIndex()),
            ]);
        }

        protected abstract zIndex GetZIndex();
    }
}
