using Guppy.Core.Common;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    public class ImGuiEngineSystem : ISceneSystem<IStrategy>, IImGuiComponent
    {
        private readonly ActionSequenceGroup<ImGuiSequenceGroupEnum, GameTime> _imGuiActions = new(true);

        [SequenceGroup<InitializeSequenceGroupEnum>(InitializeSequenceGroupEnum.PostInitialize)]
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