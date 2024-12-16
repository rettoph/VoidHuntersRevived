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

        protected override Coupling Read(in DeserializationOptions options, ref EntityReader reader, in InitializingEntity entity)
        {
            if (reader.ReadBoolean() == true)
            {
                EntityGlobalId globalNodeId = reader.ReadGlobalEntityId();
                byte index = reader.ReadByte();

                if (_entityQueryService.TryGetLocalId(globalNodeId, out EntityLocalId nodeLocalId))
                {
                    return new Coupling(
                        socketId: new NodeSocketLocalId(
                            nodeLocalId: nodeLocalId,
                            socketIndex: index)
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

        protected override void Write(ref EntityWriter writer, in Entity entity, in Coupling instance, in SerializationOptions options)
        {
            if (writer.WriteIf(instance.SocketId != NodeSocketLocalId.Empty))
            {
                EntityGlobalId nodeGlobalId = _entityQueryService.GetGlobalId(instance.SocketId.NodeLocalId);

                writer.Write(nodeGlobalId);
                writer.Write(instance.SocketId.SocketIndex);
            }
        }
    }
}
