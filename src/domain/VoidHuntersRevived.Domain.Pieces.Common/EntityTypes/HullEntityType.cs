using Guppy.Core.Serialization.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.EntityTypes
{
    [PolymorphicJsonType<IEntityType>(nameof(HullEntityType))]
    public class HullEntityType : PieceEntityType
    {
        public HullEntityType(IKey<HullEntityType> key, IKey<IEntityType>[] include) : base(key, include)
        {
            this.WithInstanceEntityComponents([
                default(Sockets<SocketId>)
            ]);

            this.RequireInstanceEntityComponents([
                typeof(Sockets<Location>),
            ]);
        }
    }
}
