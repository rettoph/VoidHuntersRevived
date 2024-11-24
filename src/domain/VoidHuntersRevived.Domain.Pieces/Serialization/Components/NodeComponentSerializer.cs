using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Components
{
    public sealed class NodeComponentSerializer(IEntityQueryService entityQueryService) : ComponentSerializer<Node>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;

        protected override Node Read(in DeserializationOptions options, ref EntityReader reader, in EntityId id)
        {
            return new Node(id, _entityQueryService.GetId(options.Owner));
        }

        protected override void Write(ref EntityWriter writer, in EntityId id, in Node instance, in SerializationOptions options)
        {
        }
    }
}
