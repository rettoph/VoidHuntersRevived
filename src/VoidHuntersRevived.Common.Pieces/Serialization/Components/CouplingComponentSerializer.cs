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
using VoidHuntersRevived.Common.Pieces.Components;

namespace VoidHuntersRevived.Common.Pieces.Serialization.Components
{
    [AutoLoad]
    public sealed class CouplingComponentSerializer : ComponentSerializer<Coupling>
    {
        private readonly IEntityService _entities;

        public CouplingComponentSerializer(IEntityService entities)
        {
            _entities = entities;
        }

        protected override Coupling Read(EntityReader reader, EntityId id)
        {
            if (reader.ReadIf())
            {
                VhId nodeVhId = reader.ReadVhId();
                byte index = reader.ReadByte();

                if (_entities.TryGetId(nodeVhId, out EntityId nodeId))
                {
                    return new Coupling(
                        socketId: new SocketId(
                            nodeId: nodeId,
                            index: index)
                        );
                }
                else
                {

                }
            }

            return default;
        }

        protected override void Write(EntityWriter writer, Coupling instance)
        {
            if (writer.WriteIf(instance.SocketId != SocketId.Empty))
            {
                writer.Write(instance.SocketId.NodeId.VhId);
                writer.Write(instance.SocketId.Index);
            }
        }
    }
}
