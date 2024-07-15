using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    internal partial class TreeService : StrategyEngine, ITreeService
    {
        private readonly IEntityQueryService _entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService;
        private readonly IBlueprintService _blueprintService;

        public TreeService(
            IEntityQueryService entityQueryService,
            IEntitySpawnService entitySpawnService,
            IBlueprintService blueprintService)
        {
            _entityQueryService = entityQueryService;
            _entitySpawnService = entitySpawnService;
            _blueprintService = blueprintService;
        }

        public ref Node GetHead(in Tree tree)
        {
            return ref _entityQueryService.QueryById<Node>(tree.HeadId);
        }

        public ref Node GetHead(in EntityId treeId)
        {
            ref Tree tree = ref _entityQueryService.QueryById<Tree>(treeId);
            return ref this.GetHead(in tree);
        }

        public ref Node GetHead(in GroupIndex treeGroupIndex)
        {
            ref Tree tree = ref _entityQueryService.QueryByGroupIndex<Tree>(treeGroupIndex);
            return ref this.GetHead(in tree);
        }
    }
}
