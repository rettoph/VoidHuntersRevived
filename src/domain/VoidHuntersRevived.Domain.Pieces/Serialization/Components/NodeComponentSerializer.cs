using Guppy.Core.Logging.Common;
using VoidHuntersRevived.Domain.Entities.Common;
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
            EntityLocalId treeLocalId = this._entityQueryService.GetLocalId(options.Owner);

            this._logger.Verbose("Deserializing Node - Id = {Id}, TreeLocalId = {TreeLocalId}", entity.GlobalId, treeLocalId);

            return new Node(entity.LocalId, treeLocalId);
        }

        protected override void Write(ref EntityWriter writer, in Entity entity, in Node instance, in SerializationOptions options)
        {
        }
    }
}