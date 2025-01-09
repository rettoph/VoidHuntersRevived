using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Game.Client.Engines.Debugging
{
    internal class StrategyDebugEngine(IImGui imgui) : StrategyEngine<Strategy>, IOnDebugEngine
    {
        private readonly IImGui _imgui = imgui;

        [SequenceGroup<DebugSequenceGroup>("Strategy")]
        public void OnDebug(GameTime gameTime)
        {
            this._imgui.KeyValue("Time", TimeSpan.FromSeconds((float)this.Strategy.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'), valueColor: Color.Cyan.ToVector4());
        }
    }
}