using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal class EntitiesDebugEngine : StrategyEngine, ISimpleDebugEngine
    {
        public const string Trees = nameof(Trees);
        public const string Nodes = nameof(Nodes);

        private readonly IEntityQueryService _entityQueryService;

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public EntitiesDebugEngine(IEntityQueryService entityQueryService)
        {
            _entityQueryService = entityQueryService;

            Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IEntityService), Trees, GetTreesValue),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IEntityService), Nodes, GetNodesValue)
            };
        }

        private string GetTreesValue()
        {
            return _entityQueryService.CalculateTotal<Tree>().ToString("#,###,##0");
        }

        private string GetNodesValue()
        {
            return _entityQueryService.CalculateTotal<Node>().ToString("#,###,##0");
        }
    }
}
