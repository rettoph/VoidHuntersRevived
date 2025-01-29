using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    public sealed class DebugEngineComponent(IImGui imgui) : ISceneComponent<IStrategy>, IDebugComponent
    {
        private readonly IImGui _imgui = imgui;
        private readonly ActionSequenceGroup<DebugSequenceGroupEnum, GameTime> _debugActions = new(true);

        [SequenceGroup<InitializeComponentSequenceGroupEnum>(InitializeComponentSequenceGroupEnum.Initialize)]
        public void Initialize(IStrategy strategy)
        {
            this._debugActions.Add(strategy.Engines);
        }

        [SequenceGroup<DebugSequenceGroupEnum>(DebugSequenceGroupEnum.Debug)]
        public void DrawDebug(GameTime gameTime)
        {
            foreach ((var group, var groupedDebugActions) in this._debugActions.Grouped)
            {
                if (this._imgui.CollapsingHeader(group.Name))
                {
                    this._imgui.Indent();

                    groupedDebugActions?.Invoke(gameTime);

                    this._imgui.Unindent();
                }
            }
        }
    }
}