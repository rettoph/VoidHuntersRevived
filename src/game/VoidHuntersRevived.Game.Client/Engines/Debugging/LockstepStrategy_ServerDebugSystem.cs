using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Game.Client.Systems.Debugging
{
    internal class LockstepStrategy_ServerDebugSystem(IImGui imgui) : StrategySystem<LockstepStrategy_Server>, IOnDebugSystem
    {
        private readonly IImGui _imgui = imgui;

        [SequenceGroup<DebugSequenceGroupEnum>("Strategy")]
        public void OnDebug(GameTime gameTime)
        {
            this._imgui.KeyValue("Tick", this.Strategy.CurrentTick.Id.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("Step", $"{this.Strategy.StepsSinceTick}/{this.Strategy.StepsPerTick}", valueColor: Color.Cyan.ToVector4());
        }
    }
}