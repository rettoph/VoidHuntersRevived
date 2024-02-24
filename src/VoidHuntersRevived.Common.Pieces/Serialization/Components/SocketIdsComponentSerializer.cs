using Guppy.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common.Core;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public class SocketIdsComponentSerializer : ComponentSerializer<Sockets<SocketId>>
    {
        private readonly IEntityService _entities;
        private readonly ISocketService _sockets;

        public SocketIdsComponentSerializer(IEntityService entities, ISocketService sockets)
        {
            _entities = entities;
            _sockets = sockets;
        }

        public override void Deserialize(in VhId sourceId, in DeserializationOptions options, EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            //
        }

        protected override void Write(EntityWriter writer, in Sockets<SocketId> instance, in SerializationOptions options)
        {
            if (options.Recursion == Recursion.None)
            {
                return;
            }

            for (int i = 0; i < instance.Items.count; i++)
            {
                this.WriteSocketCouplings(writer, instance.Items[i], options);
            }
        }

        private void WriteSocketCouplings(EntityWriter writer, SocketId socketId, SerializationOptions options)
        {
            ref var filter = ref _sockets.GetCouplingFilter(socketId);

            foreach (var (indices, groupId) in filter)
            {
                var (entityIds, _) = _entities.QueryEntities<EntityId>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    writer.Push(entityIds[indices[i]]);
                }
            }
        }

        protected override Sockets<SocketId> Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            throw new NotImplementedException();
        }
    }
}
