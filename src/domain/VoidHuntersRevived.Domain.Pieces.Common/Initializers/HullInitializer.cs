using Guppy.Core.Common.Attributes;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Providers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.EntityTypes;

namespace VoidHuntersRevived.Domain.Pieces.Common.Initializers
{
    [AutoLoad]
    internal sealed class HullInitializer : BaseEntityTypeProviderInitializer
    {
        public HullInitializer()
        {
            this.WithInstanceEntityInitializer<HullEntityType>(this.InitializeSocketIds);
        }

        private void InitializeSocketIds(IEntityService entities, IEntityTypeProvider provider, EntityId id, ref EntityInitializer initializer)
        {
            ref Sockets<Location> socketLocations = ref initializer.Get<Sockets<Location>>();
            NativeDynamicArrayCast<SocketId> socketIds = new NativeDynamicArrayCast<SocketId>((uint)socketLocations.Items.count, Allocator.Persistent);

            for (byte i = 0; i < socketLocations.Items.count; i++)
            {
                socketIds.Set(i, new SocketId(id, i));
            }

            initializer.Init<Sockets<SocketId>>(new Sockets<SocketId>()
            {
                Items = socketIds
            });
        }
    }
}
