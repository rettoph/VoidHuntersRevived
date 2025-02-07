using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Game.Client.Systems.Debugging
{
    internal class LockstepStrategy_ServerDebugSystem(
        ILockstepStrategy strategy,
        IImGui imgui
    ) : ISceneSystem,
        IDebugSystem
    {
        private readonly IImGui _imgui = imgui;

        private readonly LockstepStrategy_Server _strategy = (LockstepStrategy_Server)strategy;

        [SequenceGroup<DebugSequenceGroupEnum>("Strategy")]
        public void DrawDebug(GameTime gameTime)
        {
            this._imgui.KeyValue("Tick", this._strategy.CurrentTick.Id.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("Step", $"{this._strategy.StepsSinceTick}/{this._strategy.StepsPerTick}", valueColor: Color.Cyan.ToVector4());
        }
    }
}