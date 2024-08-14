using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;

namespace VoidHuntersRevived.Domain.Graphics.Common.Descriptors
{
    public class PrimitiveEntityType : EntityType
    {
        public PrimitiveEntityType(IKey<IEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
        }
    }
}
