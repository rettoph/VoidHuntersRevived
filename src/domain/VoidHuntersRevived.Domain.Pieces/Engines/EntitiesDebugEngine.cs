using Guppy.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components.Instance;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Pieces.Engines
{
    [AutoLoad]
    internal class EntitiesDebugEngine : BasicEngine, ISimpleDebugEngine
    {
        public const string Trees = nameof(Trees);
        public const string Nodes = nameof(Nodes);

        private readonly IEntityService _entities;

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public EntitiesDebugEngine(IEntityService entities)
        {
            _entities = entities;

            Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IEntityService), Trees, GetTreesValue),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IEntityService), Nodes, GetNodesValue)
            };
        }

        private string GetTreesValue()
        {
            return _entities.CalculateTotal<Tree>().ToString("#,###,##0");
        }

        private string GetNodesValue()
        {
            return _entities.CalculateTotal<Node>().ToString("#,###,##0");
        }
    }
}
