using Guppy.Attributes;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public class SocketsComponentSerializer : ComponentSerializer<Sockets>
    {
        private readonly IEntityService _entities;

        public SocketsComponentSerializer(IEntityService entities)
        {
            _entities = entities;
        }

        protected override Sockets Read(EntityReader reader, EntityId id)
        {
            return new Sockets()
            {
                Items = reader.ReadNativeDynamicArray<Socket>(ReadJoint),
            };
        }

        protected override void Write(EntityWriter writer, Sockets instance)
        {
            writer.WriteNativeDynamicArray(instance.Items, WriteJoint);
        }

        private Socket ReadJoint(EntityReader reader)
        {
            Socket socket = new Socket(
                nodeId: _entities.GetId(reader.ReadVhId()),
                index: reader.ReadByte(),
                location: reader.ReadStruct<Location>());

            if (reader.ReadIf())
            {
                socket.PlugId = _entities.Deserialize(reader, null);
            }

            return socket;
        }

        private void WriteJoint(EntityWriter writer, Socket joint)
        {
            writer.Write(joint.Id.NodeId.VhId);
            writer.Write(joint.Id.Index);
            writer.WriteStruct<Location>(joint.Location);

            if (writer.WriteIf(joint.PlugId.VhId.Value != default))
            {
                _entities.Serialize(joint.PlugId, writer);
            }
        }
    }
}
