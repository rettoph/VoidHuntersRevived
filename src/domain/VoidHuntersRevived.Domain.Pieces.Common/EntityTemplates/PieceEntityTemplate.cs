using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Teams.Common.EntityTemplates;

namespace VoidHuntersRevived.Domain.Pieces.Common.EntityTemplates
{
    [PolymorphicJsonType<IEntityTemplate>(nameof(PieceEntityTemplate))]
    public abstract class PieceEntityTemplate : VisibleTeamMemberEntityTemplate
    {
        public PieceEntityTemplate(Key<IEntityTemplate> key) : base(key)
        {
            this.WithComponents([
                Plug.Default,
                new Coupling(),
                new Node(),
                new BelongsTo<Tree, Node>(),
                new VertexVisible()
            ]);

            this.RequireComponents([
                typeof(Rigid),
                typeof(PrimitiveComponent<VertexVisible>)
            ]);
        }
    }
}
