using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Engines.Debug
{
    [AutoLoad]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal class EntitiesDebugEngine : StrategyEngine, ISimpleDebugEngine
    {
        public const string Entities = nameof(Entities);

        private readonly IEntityService _entities;

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public EntitiesDebugEngine(IEntityService entities)
        {
            _entities = entities;

            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IEntityService), Entities, () => _entities.CalculateTotal<EntityId>().ToString("#,###,##0"))
            };
        }
    }
}
