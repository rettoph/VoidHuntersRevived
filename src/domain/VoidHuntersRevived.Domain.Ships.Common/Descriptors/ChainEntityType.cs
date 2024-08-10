using Guppy.Core.Common.Attributes;
using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Pieces.Common.EntityTypes;
using VoidHuntersRevived.Domain.Ships.Common.Components;

namespace VoidHuntersRevived.Domain.Ships.Common.Descriptors
{
    [AutoLoad]
    [PolymorphicJsonType<IEntityType>(nameof(ChainEntityType))]
    public class ChainEntityType : TreeEntityType
    {
        public static readonly IKey<ChainEntityType> ChainEntityTypeKey = VoidHuntersRevived.Common.Key.GetByName<ChainEntityType>(nameof(ChainEntityType));

        public ChainEntityType() : base(ChainEntityType.ChainEntityTypeKey, Array.Empty<IKey<IEntityType>>())
        {
            this.WithInstanceEntityComponents([
                new Tractorable()
            ]);
        }
    }
}
