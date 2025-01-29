using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common.Components;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    public class ImGuiEngineComponent : ISceneComponent<IStrategy>, IImGuiComponent
    {
        private readonly ActionSequenceGroup<ImGuiSequenceGroupEnum, GameTime> _imGuiActions = new(true);

        [SequenceGroup<InitializeComponentSequenceGroupEnum>(InitializeComponentSequenceGroupEnum.PostInitialize)]
        public void Initialize(IStrategy strategy)
        {
            this._imGuiActions.Add(strategy.Engines);
        }

        [SequenceGroup<ImGuiSequenceGroupEnum>(ImGuiSequenceGroupEnum.PostDraw)]
        public void DrawImGui(GameTime gameTime)
        {
            this._imGuiActions.Invoke(gameTime);
        }
    }
}