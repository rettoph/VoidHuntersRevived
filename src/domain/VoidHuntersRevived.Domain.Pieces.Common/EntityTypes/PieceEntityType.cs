using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Graphics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Static;
using VoidHuntersRevived.Domain.Pieces.Common.Graphics.Vertices;
using VoidHuntersRevived.Domain.Teams.Common.EntityTypes;

namespace VoidHuntersRevived.Domain.Pieces.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(PieceEntityType))]
    public abstract class PieceEntityType : VisibleTeamMemberEntityType
    {
        public PieceEntityType(IKey<IEntityType> key) : base(key)
        {
            this.WithComponents([
                Plug.Default,
                new Coupling(),
                new Node(),
                new BelongsTo<Tree, Node>(),
                new PrimitiveGroup(),
                new VertexVisible()
            ]);

            this.RequireComponents([
                typeof(Rigid),
                typeof(Visible)
            ]);
        }
    }
}
