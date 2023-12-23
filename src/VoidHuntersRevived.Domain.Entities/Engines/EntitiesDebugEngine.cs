using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Entities.Engines.Debug
{
    [AutoLoad]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal class EntitiesDebugEngine : BasicEngine, ISimpleDebugEngine
    {
        public const string Entities = nameof(Entities);

        private readonly IEntityService _entities;

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public EntitiesDebugEngine(IEntityService entities, IImGui imgui)
        {
            _entities = entities;

            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IEntityService), Entities, () => _entities.CalculateTotal<EntityId>().ToString("#,###,##0"))
            };
        }
    }
}
