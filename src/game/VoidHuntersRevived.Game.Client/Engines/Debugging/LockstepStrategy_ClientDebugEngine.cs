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
    [AutoLoad]
    [StrategyFilter<LockstepStrategy_Client>]
    internal class LockstepStrategy_ClientDebugEngine : StrategyEngine<LockstepStrategy_Client>, IOnDebugEngine
    {
        private readonly IImGui _imgui;

        public LockstepStrategy_ClientDebugEngine(IImGui imgui)
        {
            _imgui = imgui;
        }

        [SequenceGroup<DebugSequenceGroup>("Strategy")]
        public void OnDebug(GameTime gameTime)
        {
            _imgui.KeyValue("Tick", this.Strategy.CurrentTick.Id.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("Step", $"{this.Strategy.StepsSinceTick}/{this.Strategy.StepsPerTick}", valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("BufferHead", $"{(this.Strategy.TickBuffer.Head?.Id.ToString()) ?? "null"}", valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("BufferTail", $"{(this.Strategy.TickBuffer.Tail?.Id.ToString()) ?? "null"}", valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("BufferCount", this.Strategy.TickBuffer.Count.ToString("#,##0"), valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("TimeSinceStep", this.Strategy.TimeSinceStep.ToString(@"hh\:mm\:ss\.FFFFFFF"), valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("StepTimespan", this.Strategy.StepTimeSpan.ToString(@"hh\:mm\:ss\.FFFFFFF"), valueColor: Color.Cyan.ToVector4());
        }
    }
}
