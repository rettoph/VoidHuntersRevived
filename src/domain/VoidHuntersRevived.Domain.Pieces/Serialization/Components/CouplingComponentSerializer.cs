using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Components
{
    public sealed class CouplingComponentSerializer(IEntityQueryService entityQueryService) : ComponentSerializer<Coupling>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        protected override Coupling Read(in DeserializationOptions options, ref EntityReader reader, in EntityId id)
        {
            if (reader.ReadBoolean() == true)
            {
                VhId nodeVhId = reader.ReadVhId();
                byte index = reader.ReadByte();

                if (_entityQueryService.TryGetId(nodeVhId, out EntityId nodeId))
                {
                    return new Coupling(
                        socketId: new NodeSocketId(
                            nodeId: nodeId,
                            index: index)
                        );
                }
                else
                {
                    // Currently this can happen organically if we serialize a partial tree
                    // The new root will serialize with the owner info but when deserializing
                    // with a new seed the 'old' owner wont exist. Can this be fixed?
                    // TODO: Do nothing? Throw? This needs to be refactored somehow
                }
            }

            return default;
        }

        protected override void Write(ref EntityWriter writer, in EntityId id, in Coupling instance, in SerializationOptions options)
        {
            if (writer.WriteIf(instance.SocketId != NodeSocketId.Empty))
            {
                writer.Write(instance.SocketId.NodeId.VhId);
                writer.Write(instance.SocketId.Index);
            }
        }
    }
}
