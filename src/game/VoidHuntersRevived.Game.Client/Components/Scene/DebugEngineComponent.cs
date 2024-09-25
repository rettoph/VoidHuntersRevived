using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    [AutoLoad]
    [SceneFilter<IStrategy>]
    internal sealed class DebugEngineComponent : ISceneComponent<IStrategy>, IDebugComponent
    {
        private readonly IImGui _imgui;
        private readonly ActionSequenceGroup<DebugSequenceGroup, GameTime> _debugActions;

        public DebugEngineComponent(IImGui imgui)
        {
            _imgui = imgui;
            _debugActions = new ActionSequenceGroup<DebugSequenceGroup, GameTime>();
        }


        [SequenceGroup<InitializeComponentSequenceGroup>(InitializeComponentSequenceGroup.Initialize)]
        public void Initialize(IStrategy strategy)
        {
            _debugActions.Add(strategy.Engines);
        }

        [SequenceGroup<DebugSequenceGroup>(DebugSequenceGroup.Debug)]
        public void DrawDebug(GameTime gameTime)
        {
            foreach ((var group, var groupedDebugActions) in _debugActions.Grouped)
            {
                if (_imgui.CollapsingHeader(group.Name))
                {
                    _imgui.Indent();

                    groupedDebugActions?.Invoke(gameTime);

                    _imgui.Unindent();
                }
            }
        }
    }
}
