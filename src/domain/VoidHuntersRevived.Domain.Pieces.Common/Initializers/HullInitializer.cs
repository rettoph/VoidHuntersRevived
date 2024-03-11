using Guppy.Attributes;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Initializers;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Physics.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Descriptors;

namespace VoidHuntersRevived.Domain.Pieces.Common.Initializers
{
    [AutoLoad]
    internal sealed class HullInitializer : BaseEntityInitializer
    {
        public HullInitializer()
        {
            this.WithInstanceInitializer<HullDescriptor>(this.InitializeSocketIds);
        }

        private void InitializeSocketIds(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
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
