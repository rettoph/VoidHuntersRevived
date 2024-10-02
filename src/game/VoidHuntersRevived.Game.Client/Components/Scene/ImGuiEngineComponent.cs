using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    [AutoLoad]
    [SceneFilter<IStrategy>]
    internal class ImGuiEngineComponent : ISceneComponent<IStrategy>, IImGuiComponent
    {
        private readonly ActionSequenceGroup<ImGuiSequenceGroup, GameTime> _imGuiActions = new();

        [SequenceGroup<InitializeComponentSequenceGroup>(InitializeComponentSequenceGroup.PostInitialize)]
        public void Initialize(IStrategy strategy)
        {
            _imGuiActions.Add(strategy.Engines);
        }

        [SequenceGroup<ImGuiSequenceGroup>(ImGuiSequenceGroup.PostDraw)]
        public void DrawImGui(GameTime gameTime)
        {
            _imGuiActions.Invoke(gameTime);
        }
    }
}
