using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Client.Systems.Debugging
{
    public class StrategyDebugSystem(IStrategy strategy, IImGui imgui) :
        ISceneSystem,
        IDebugSystem
    {
        private readonly IImGui _imgui = imgui;
        private readonly Strategy _strategy = (Strategy)strategy;

        [SequenceGroup<DebugSequenceGroupEnum>("Strategy")]
        public void DrawDebug(GameTime gameTime)
        {
            this._imgui.KeyValue("Time", TimeSpan.FromSeconds((float)this._strategy.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'), valueColor: Color.Cyan.ToVector4());
        }
    }
}