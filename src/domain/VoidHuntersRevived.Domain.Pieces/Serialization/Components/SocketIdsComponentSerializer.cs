using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Components
{
    public class SocketIdsComponentSerializer(IEntityQueryService entityQueryService, INodeSocketService socketService) : ComponentSerializer<Sockets>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly INodeSocketService _socketService = socketService;

        protected override void Write(ref EntityWriter writer, in Entity entity, in Sockets instance, in SerializationOptions options)
        {
            if (options.Recursion == Recursion.None)
            {
                return;
            }

            for (int i = 0; i < instance.Items.count; i++)
            {
                this.WriteSocketCouplings(ref writer, in entity, (byte)i, options);
            }
        }

        private void WriteSocketCouplings(ref EntityWriter writer, in Entity entity, byte socketIndex, SerializationOptions options)
        {
            ref var filter = ref _socketService.GetCouplingFilter(entity.EntityId, socketIndex);

            foreach (var (indices, groupId) in filter)
            {
                var (entityLocalIds, _) = _entityQueryService.QueryEntities<EntityLocalId>(groupId);

                for (int i = 0; i < indices.count; i++)
                {
                    writer.Push(entityLocalIds[indices[i]]);
                }
            }
        }

        public override void Deserialize(in VhId sourceId, in DeserializationOptions options, ref EntityReader reader, in InitializingEntity entity)
        {
            // No deserialization needed
            // base.Deserialize(sourceId, options, reader, ref initializer, id);
        }

        protected override Sockets Read(in DeserializationOptions options, ref EntityReader reader, in InitializingEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
