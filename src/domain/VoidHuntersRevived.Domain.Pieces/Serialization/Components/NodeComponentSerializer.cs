using Serilog;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Extensions;
using VoidHuntersRevived.Domain.Entities.Common.Options;
using VoidHuntersRevived.Domain.Entities.Common.Serialization;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Serialization.Components
{
    public sealed class NodeComponentSerializer(IEntityQueryService entityQueryService, ILogger logger) : ComponentSerializer<Node>
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly ILogger _logger = logger;
        protected override Node Read(in DeserializationOptions options, ref EntityReader reader, in InitializingEntity entity)
        {
            EntityId treeId = _entityQueryService.GetId(options.Owner);

            _logger.Verbose("Deserializing Node - Id = {Id}, TreeId = {TreeId}, TreeLocalId = {TreeLocalId}", entity.GlobalId, treeId.VhId, treeId.ToLocalEntityId());

            return new Node(entity.EntityId, treeId.ToLocalEntityId());
        }

        protected override void Write(ref EntityWriter writer, in Entity entity, in Node instance, in SerializationOptions options)
        {
        }
    }
}
