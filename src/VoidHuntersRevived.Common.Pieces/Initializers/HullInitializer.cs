using Guppy.Attributes;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Initializers;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components.Instance;
using VoidHuntersRevived.Common.Pieces.Descriptors;

namespace VoidHuntersRevived.Common.Pieces.Initializers
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
