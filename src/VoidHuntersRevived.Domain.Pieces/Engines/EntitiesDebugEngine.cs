using Guppy.Attributes;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations.Engines;

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
