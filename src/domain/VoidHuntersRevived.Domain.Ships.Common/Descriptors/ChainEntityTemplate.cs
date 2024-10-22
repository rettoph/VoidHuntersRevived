using Guppy.Core.Common.Attributes;
using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Constants;
using VoidHuntersRevived.Domain.Pieces.Common.EntityTemplates;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    [AutoLoad]
    [PolymorphicJsonType<IEntityTemplate>(nameof(ChainEntityTemplate))]
    public class ChainEntityTemplate : TreeEntityTemplate
    {
        public static readonly Key<IEntityTemplate> ChainEntityTemplateKey = Key<IEntityTemplate>.GetByName(nameof(ChainEntityTemplate));

        public ChainEntityTemplate() : base(ChainEntityTemplate.ChainEntityTemplateKey)
        {
            this.WithComponents([
                new Tractorable(),
                new Collision()
                {
                    Categories = CollisionGroups.FreeFloatingCategories,
                    CollidesWith = CollisionGroups.FreeFloatingCollidesWith
                }
            ]);
        }
    }
}
