using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Entities.Engines.Debug
{
    [AutoLoad]
    [SequenceGroup<DrawSequence>(DrawSequence.PreDraw)]
    internal class EntitiesDebugEngine : StrategyEngine, ISimpleDebugEngine
    {
        public const string Entities = nameof(Entities);

        private readonly IEntityQueryService _entityQueryService;

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public EntitiesDebugEngine(IEntityQueryService entityQueryService)
        {
            _entityQueryService = entityQueryService;

            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IEntityService), Entities, () => _entityQueryService.CalculateTotal<EntityId>().ToString("#,###,##0"))
            };
        }
    }
}
