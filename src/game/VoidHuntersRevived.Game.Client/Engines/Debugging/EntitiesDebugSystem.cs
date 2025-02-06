using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Pieces.Common.Components;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;

namespace VoidHuntersRevived.Game.Client.Systems.Debugging
{
    internal class EntitiesDebugSystem(IImGui imgui, IEntityQueryService entityQueryService) : StrategySystem, IOnDebugSystem
    {
        private readonly IEntityQueryService _entityQueryService = entityQueryService;
        private readonly IImGui _imgui = imgui;

        [SequenceGroup<DebugSequenceGroupEnum>("Entities")]
        public void OnDebug(GameTime gameTime)
        {
            this._imgui.KeyValue("Total", this._entityQueryService.CalculateTotal<EntityLocalId>().ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("Trees", this._entityQueryService.CalculateTotal<Tree>().ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("Nodes", this._entityQueryService.CalculateTotal<Node>().ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
        }
    }
}