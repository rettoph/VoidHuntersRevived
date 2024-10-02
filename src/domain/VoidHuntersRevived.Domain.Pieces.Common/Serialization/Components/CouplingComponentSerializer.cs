using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;

namespace VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components
{
    [AutoLoad]
    public sealed class CouplingComponentSerializer(IEntityQueryService entityQueryService) : ComponentSerializer<Coupling>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        protected override Coupling Read(in DeserializationOptions options, EntityReader reader, in EntityId id)
        {
            if (reader.ReadIf())
            {
                VhId nodeVhId = reader.ReadVhId(options.Seed);
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

                }
            }

            return default;
        }

        protected override void Write(EntityWriter writer, in EntityId id, in Coupling instance, in SerializationOptions options)
        {
            if (writer.WriteIf(instance.SocketId != NodeSocketId.Empty))
            {
                writer.Write(instance.SocketId.NodeId.VhId);
                writer.Write(instance.SocketId.Index);
            }
        }
    }
}
