using Guppy.Core.Common.Attributes;
using Serilog;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    public sealed class TreeEngine(
        IEntitySpawnService entitySpawnService,
        ILogger logger) : StrategyEngine,
        IOnDespawnEngine<Tree>
    {
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;
        private readonly ILogger _logger = logger;

        [SequenceGroup<OnDespawnSequenceGroupEnum>(OnDespawnSequenceGroupEnum.Group03)]
        public void OnDespawn(VhId sourceEventId, IEntityTemplate template, ref Entity<Tree> tree)
        {
            _logger.Verbose("Despawning Tree {TreeId}, HeadLocalId = {HeadLocalId}", tree.LocalId, tree.Component.HeadLocalId);
            _entitySpawnService.Despawn(sourceEventId, tree.Component.HeadLocalId);
        }
    }
}
