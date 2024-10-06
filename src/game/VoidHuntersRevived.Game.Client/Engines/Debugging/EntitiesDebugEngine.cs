using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Game.Client.Engines.Debugging
{
    [AutoLoad]
    internal class EntitiesDebugEngine(IImGui imgui, IEntityQueryService entityQueryService) : StrategyEngine, IOnDebugEngine
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IImGui _imgui = imgui;

        [SequenceGroup<DebugSequenceGroup>("Entities")]
        public void OnDebug(GameTime gameTime)
        {
            _imgui.KeyValue("Total", _entityQueryService.CalculateTotal<EntityId>().ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("Trees", _entityQueryService.CalculateTotal<Tree>().ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("Nodes", _entityQueryService.CalculateTotal<Node>().ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
        }
    }
}
