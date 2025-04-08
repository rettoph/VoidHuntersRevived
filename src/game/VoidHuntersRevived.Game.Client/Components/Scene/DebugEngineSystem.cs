using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Core.Common.Systems;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.ImGui.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common.Strategies;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    public sealed class DebugEngineSystem(
        IStrategy strategy,
        IImGui imgui
    ) : ISceneSystem,
        IInitializeSystem,
        IDeinitializeSystem,
        IDebugSystem
    {
        private readonly IStrategy _strategy = strategy;
        private readonly IImGui _imgui = imgui;
        private readonly ActionSequenceGroup<DebugSequenceGroupEnum, GameTime> _debugActions = new(true);

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.Initialize)]
        public void Initialize()
        {
            this._debugActions.Add(this._strategy.Systems.GetAll());
        }

        [SequenceGroup<DeinitializeSequenceGroupEnum>(DeinitializeSequenceGroupEnum.PreInitialize)]
        public void Deinitialize()
        {
            this._debugActions.Remove(this._strategy.Systems.GetAll());
        }

        [SequenceGroup<DebugSequenceGroupEnum>(DebugSequenceGroupEnum.Debug)]
        public void DrawDebug(GameTime gameTime)
        {
            foreach ((var group, var groupedDebugActions) in this._debugActions.Grouped)
            {
                if (group == SequenceGroup<DebugSequenceGroupEnum>.GetByValue(DebugSequenceGroupEnum.Debug))
                {
                    continue;
                }

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