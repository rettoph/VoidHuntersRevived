using Guppy.Attributes;
using Svelto.ECS;
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
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                _entities.Deserialize(sourceId, options, reader, null);
            }
        }

        protected override void Write(EntityWriter writer, in Sockets<SocketId> instance, in SerializationOptions options)
        {
            var start = writer.BaseStream.Position;
            writer.Write(0);

            if (options.Recursion == Recursion.None)
            {
                return;
            }

            int count = 0;
            for (int i = 0; i < instance.Items.count; i++)
            {
                count += this.WriteSocketCouplings(writer, instance.Items[i], options);
            }

            var end = writer.BaseStream.Position;

            writer.BaseStream.Position = start;
            writer.Write(count);
            writer.BaseStream.Position = end;
        }

        private int WriteSocketCouplings(EntityWriter writer, SocketId socketId, SerializationOptions options)
        {
            var filter = _sockets.GetCouplingFilter(socketId);
            int count = 0;

            foreach (var (indices, groupId) in filter)
            {
                var (entityIds, _) = _entities.QueryEntities<EntityId>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    count++;

                    _entities.Serialize(entityIds[indices[i]], writer, options);
                }
            }

            return count;
        }

        protected override Sockets<SocketId> Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            throw new NotImplementedException();
        }
    }
}
