using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Serialization.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public class HullDescriptor : PieceDescriptor
    {
        public HullDescriptor() : base(Resources.Colors.HullPrimaryColor, Resources.Colors.HullSecondaryColor, 0)
        {
            this.WithInstanceComponents(new ComponentManager[]
            {
                //new ComponentManager<Sockets, SocketsComponentSerializer>(),
                new ComponentManager<Sockets<Location>, SocketLocationsComponentSerializer>(),
                new ComponentManager<Sockets<SocketId>, SocketIdsComponentSerializer>(),
            });

            this.WithPostInitializer(this.BuildSocketIds);
        }

        private void BuildSocketIds(IEntityService entities, ref EntityInitializer initializer, in EntityId id)
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
