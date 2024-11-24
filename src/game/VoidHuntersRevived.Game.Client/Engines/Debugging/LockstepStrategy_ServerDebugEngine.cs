using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Game.Client.Engines.Debugging
{
    [StrategyFilter<LockstepStrategy_Server>]
    internal class LockstepStrategy_ServerDebugEngine(IImGui imgui) : StrategyEngine<LockstepStrategy_Server>, IOnDebugEngine
    {
        private readonly IImGui _imgui = imgui;

        [SequenceGroup<DebugSequenceGroup>("Strategy")]
        public void OnDebug(GameTime gameTime)
        {
            _imgui.KeyValue("Tick", this.Strategy.CurrentTick.Id.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("Step", $"{this.Strategy.StepsSinceTick}/{this.Strategy.StepsPerTick}", valueColor: Color.Cyan.ToVector4());
        }
    }
}
