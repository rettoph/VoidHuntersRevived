using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Common.Serialization.Components
{
    [AutoLoad]
    public class SocketIdsComponentSerializer(IEntityQueryService entityQueryService, ISocketService socketService) : ComponentSerializer<Sockets>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ISocketService _socketService = socketService;

        protected override void Write(ref EntityWriter writer, in EntityId id, in Sockets instance, in SerializationOptions options)
        {
            if (options.Recursion == Recursion.None)
            {
                return;
            }

            for (int i = 0; i < instance.Items.count; i++)
            {
                this.WriteSocketCouplings(ref writer, in id, (byte)i, options);
            }
        }

        private void WriteSocketCouplings(ref EntityWriter writer, in EntityId nodeId, byte socketIndex, SerializationOptions options)
        {
            ref var filter = ref _socketService.GetCouplingFilter(nodeId, socketIndex);

            foreach (var (indices, groupId) in filter)
            {
                var (entityIds, _) = _entityQueryService.QueryEntities<EntityId>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    writer.Push(entityIds[indices[i]]);
                }
            }
        }

        public override void Deserialize(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, ref EntityInitializer initializer, in EntityId id)
        {
            // No deserialization needed
            // base.Deserialize(sourceId, options, reader, ref initializer, id);
        }

        protected override Sockets Read(in DeserializationOptions options, ref EntityReader reader, in EntityId id)
        {
            throw new NotImplementedException();
        }
    }
}
