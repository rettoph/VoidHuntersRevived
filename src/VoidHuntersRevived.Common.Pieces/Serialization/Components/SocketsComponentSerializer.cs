using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Enums;
using VoidHuntersRevived.Common.Entities.Options;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Pieces.Services;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public class SocketsComponentSerializer : ComponentSerializer<Sockets>
    {
        private readonly IEntityService _entities;
        private readonly ISocketService _sockets;

        public SocketsComponentSerializer(IEntityService entities, ISocketService sockets)
        {
            _entities = entities;
            _sockets = sockets;
        }

        protected override Sockets Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            return new Sockets()
            {
                Items = reader.ReadNativeDynamicArray<Socket>(ReadSocket, in options),
            };
        }

        protected override void Write(EntityWriter writer, in Sockets instance, in SerializationOptions options)
        {
            writer.WriteNativeDynamicArray(instance.Items, WriteSocket, in options);
        }

        private Socket ReadSocket(DeserializationOptions options, EntityReader reader)
        {
            Socket socket = new Socket(
                nodeId: _entities.GetId(reader.ReadVhId(options.Seed)),
                index: reader.ReadByte(),
                location: reader.ReadStruct<Location>());

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                _entities.Deserialize(options, reader, null);
            }

            return socket;
        }

        private void WriteSocket(EntityWriter writer, Socket socket, SerializationOptions options)
        {
            writer.Write(socket.Id.NodeId.VhId);
            writer.Write(socket.Id.Index);
            writer.WriteStruct<Location>(socket.Location);

            var start = writer.BaseStream.Position;
            writer.Write(0);

            if (options.Recursion == Recursion.None)
            {
                return;
            }

            var filter = _sockets.GetCouplingFilter(socket.Id);
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

            var end = writer.BaseStream.Position;

            writer.BaseStream.Position = start;
            writer.Write(count);
            writer.BaseStream.Position = end;
        }
    }
}
