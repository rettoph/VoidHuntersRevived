using Microsoft.Xna.Framework;
using Svelto.Common;
using Svelto.DataStructures;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Components;
using VoidHuntersRevived.Common.Entities.Descriptors;
using VoidHuntersRevived.Common.Entities.Serialization;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Physics.Components;
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Descriptors
{
    public class HullDescriptor : PieceDescriptor
    {
        public HullDescriptor()
        {
            this.ExtendWith(new ComponentManager[]
            {
                new ComponentManager<Sockets>(
                    builder: new ComponentBuilder<Sockets>(),
                    serializer: new ComponentSerializer<Sockets>(
                        writer: (entities, writer, instance) =>
                        {
                            writer.WriteNativeDynamicArray(entities, instance.Items, WriteJoint);
                        },
                        reader: (entities, reader, id) => new Sockets()
                        {
                            Items = reader.ReadNativeDynamicArray<Socket>(entities, ReadJoint),
                        }))
            });
        }
        private static Socket ReadJoint(IEntityService entities, EntityReader reader)
        {
            Socket socket = new Socket(
                nodeId: entities.GetId(reader.ReadVhId()),
                index: reader.ReadByte(),
                location: reader.ReadStruct<Location>());

            if(reader.ReadIf())
            {
                socket.PlugId = entities.Deserialize(reader, null);
            }

            return socket;
        }

        private static void WriteJoint(IEntityService entities, EntityWriter writer, Socket joint)
        {
            writer.Write(joint.Id.NodeId.VhId);
            writer.Write(joint.Id.Index);
            writer.WriteStruct<Location>(joint.Location);
            
            if(writer.WriteIf(joint.PlugId.VhId.Value != default))
            {
                entities.Serialize(joint.PlugId, writer);
            }
        }
    }
}
