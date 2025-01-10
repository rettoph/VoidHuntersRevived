using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Pieces.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Services
{
    public partial class TreeService(
        IEntityQueryService entityQueryService,
        IEntitySpawnService entitySpawnService) : StrategyEngine, ITreeService
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IEntitySpawnService _entitySpawnService = entitySpawnService;

        public ref Node GetHead(Tree tree) => ref this._entityQueryService.QueryByLocalId<Node>(tree.HeadLocalId);

        public ref Node GetHead(EntityLocalId treeLocalId)
        {
            ref Tree tree = ref this._entityQueryService.QueryByLocalId<Tree>(treeLocalId);
            return ref this.GetHead(tree);
        }

        public ref Node GetHead(GroupIndex treeGroupIndex)
        {
            ref Tree tree = ref this._entityQueryService.QueryByGroupIndex<Tree>(treeGroupIndex);
            return ref this.GetHead(tree);
        }
    }
}