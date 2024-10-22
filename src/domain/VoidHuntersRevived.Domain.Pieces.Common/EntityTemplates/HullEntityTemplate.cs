using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.EntityTemplates
{
    [PolymorphicJsonType<IEntityTemplate>(nameof(HullEntityTemplate))]
    public class HullEntityTemplate : PieceEntityTemplate
    {
        public HullEntityTemplate(Key<IEntityTemplate> key) : base(key)
        {
            this.RequireComponents([
                typeof(Sockets),
            ]);
        }
    }
}
