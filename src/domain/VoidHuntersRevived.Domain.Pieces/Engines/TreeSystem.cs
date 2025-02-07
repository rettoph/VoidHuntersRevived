using Guppy.Core.Common.Attributes;
using Guppy.Core.Logging.Common;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Entities.Common.Systems;
using VoidHuntersRevived.Domain.Pieces.Common.Components;

namespace VoidHuntersRevived.Domain.Pieces.Systems
{
    public sealed class TreeSystem(
        IEntitySpawnService entitySpawnService,
        ILogger logger
    ) : ISceneSystem,
        IOnDespawnSystem<Tree>
    {
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Tree> tree)
        {
            this._logger.Verbose("Despawning Tree {TreeId}, HeadLocalId = {HeadLocalId}", tree.LocalId, tree.Component.HeadLocalId);
            this._entitySpawnService.Despawn(sourceEventId, tree.Component.HeadLocalId);
        }
    }
}