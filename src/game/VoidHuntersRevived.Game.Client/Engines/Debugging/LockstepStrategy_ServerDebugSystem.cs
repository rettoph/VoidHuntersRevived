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
    internal class LockstepStrategy_ServerDebugSystem(
        IImGui imgui
    ) : ISceneSystem,
        IInitializeSystem<LockstepStrategy_Server>,
        IDebugSystem
    {
        private readonly IImGui _imgui = imgui;

        private LockstepStrategy_Server _strategy = null!;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize(LockstepStrategy_Server strategy)
        {
            this._strategy = strategy;
        }

        [SequenceGroup<DebugSequenceGroupEnum>("Strategy")]
        public void DrawDebug(GameTime gameTime)
        {
            this._imgui.KeyValue("Tick", this._strategy.CurrentTick.Id.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            this._imgui.KeyValue("Step", $"{this._strategy.StepsSinceTick}/{this._strategy.StepsPerTick}", valueColor: Color.Cyan.ToVector4());
        }
    }
}