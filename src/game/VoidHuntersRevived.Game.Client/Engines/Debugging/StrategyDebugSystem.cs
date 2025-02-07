using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Extensions;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations;

namespace VoidHuntersRevived.Game.Client.Systems.Debugging
{
    public class StrategyDebugSystem(IImGui imgui) :
        ISceneSystem,
        IInitializeSystem<Strategy>,
        IDebugSystem
    {
        private readonly IImGui _imgui = imgui;
        private Strategy _strategy = null!;

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Setup)]
        public void Initialize(Strategy strategy)
        {
            this._strategy = strategy;
        }

        [SequenceGroup<DebugSequenceGroupEnum>("Strategy")]
        public void DrawDebug(GameTime gameTime)
        {
            this._imgui.KeyValue("Time", TimeSpan.FromSeconds((float)this._strategy.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'), valueColor: Color.Cyan.ToVector4());
        }
    }
}