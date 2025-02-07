using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Game.Client.Systems.Debugging
{
    internal class LockstepStrategy_ClientDebugSystem(IImGui imgui) :
        ISceneSystem,
        IInitializeSystem<LockstepStrategy_Client>,
        IDebugSystem
    {
        private readonly IImGui _imgui = imgui;

        private LockstepStrategy_Client _strategy = null!;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize(LockstepStrategy_Client strategy)
        {
            this._strategy = strategy;
        }

        [SequenceGroup<DebugSequenceGroupEnum>("Strategy")]
        public void DrawDebug(GameTime gameTime)
        {
            this._imgui.KeyValue("Tick", this._strategy.CurrentTick.Id.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("Step", $"{this._strategy.StepsSinceTick}/{this._strategy.StepsPerTick}", valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("BufferHead", $"{(this._strategy.TickBuffer.Head?.Id.ToString()) ?? "null"}", valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("BufferTail", $"{(this._strategy.TickBuffer.Tail?.Id.ToString()) ?? "null"}", valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("BufferCount", this._strategy.TickBuffer.Count.ToString("#,##0"), valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("TimeSinceStep", this._strategy.TimeSinceStep.ToString(@"hh\:mm\:ss\.FFFFFFF"), valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("StepTimespan", this._strategy.StepTimeSpan.ToString(@"hh\:mm\:ss\.FFFFFFF"), valueColor: Color.Cyan.ToVector4());
        }
    }
}