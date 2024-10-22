using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Common.EntityTemplates
{
    [PolymorphicJsonType<IEntityTemplate>(nameof(ThrusterEntityTemplate))]
    public class ThrusterEntityTemplate : PieceEntityTemplate
    {
        public ThrusterEntityTemplate(Key<IEntityTemplate> key) : base(key)
        {
            this.WithComponents([
                new Thrustable()
            ]);
        }
    }
}
