using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Components;
using VoidHuntersRevived.Domain.Physics.Common.EntityTypes;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(TreeEntityType))]
    public abstract class TreeEntityType : BodyEntityType
    {
        public TreeEntityType(IKey<IEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
            this.WithComponents([
                new Tree(),
                new HasMany<Node, Tree>()
            ]);
        }
    }
}
