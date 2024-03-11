using Guppy.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components
{
    [AutoLoad]
    public sealed class CouplingComponentSerializer : ComponentSerializer<Coupling>
    {
        private readonly IEntityService _entities;

        public CouplingComponentSerializer(IEntityService entities)
        {
            _entities = entities;
        }

        protected override Coupling Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            if (reader.ReadIf())
            {
                VhId nodeVhId = reader.ReadVhId(options.Seed);
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

        protected override void Write(EntityWriter writer, in Coupling instance, in SerializationOptions options)
        {
            if (writer.WriteIf(instance.SocketId != SocketId.Empty))
            {
                writer.Write(instance.SocketId.NodeId.VhId);
                writer.Write(instance.SocketId.Index);
            }
        }
    }
}
