using Guppy.Core.Common.Attributes;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Game.Common.Systems;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Enums;
using Guppy.Game.ImGui.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Game.Client.Systems.Debugging
{
    public class LockstepStrategyDebugSystem(
        ILockstepStrategy strategy,
        IImGui imgui,
        IImGuiObjectExplorerService objectExplorer,
        IScene guppy
    ) : ISceneSystem,
        IImGuiSystem,
        IDebugSystem
    {
        public string? Group => nameof(IStrategy);

        private readonly IImGui _imgui = imgui;
        private readonly IImGuiObjectExplorerService _objectExplorer = objectExplorer;
        private readonly IScene _scene = guppy;
        private readonly ILockstepStrategy _strategy = strategy;
        private bool _historyViewerEnabled;
        private string _filter = string.Empty;

        [SequenceGroup<DebugSequenceGroupEnum>("Strategy")]
        public void DrawDebug(GameTime gameTime)
        {
            var buttonStyle = this._historyViewerEnabled ? Guppy.Game.MonoGame.Common.Resources.ImGuiStyles.ButtonGreen : Guppy.Game.MonoGame.Common.Resources.ImGuiStyles.ButtonRed;

            using (this._imgui.Apply(buttonStyle))
            {
                if (this._imgui.Button($"{(this._historyViewerEnabled ? "Disable" : "Enable")} Tick History Explorer"))
                {
                    this._historyViewerEnabled = !this._historyViewerEnabled;
                }
            }
        }

        [SequenceGroup<ImGuiSequenceGroupEnum>(ImGuiSequenceGroupEnum.Draw)]
        public void DrawImGui(GameTime gameTime)
        {
            if (this._historyViewerEnabled == false)
            {
                return;
            }

            this._imgui.Begin($"Tick History Explorer - {this._strategy.Type}, {this._scene.Name} {this._scene.Id}", ref this._historyViewerEnabled);
            this._imgui.InputText("Filter", ref this._filter, 255);

            using (this._imgui.ApplyID(nameof(ILockstepStrategy.History)))
            {
                this._objectExplorer.DrawObjectExplorer(this._strategy.History, this._filter, 8);
            }

            this._imgui.End();
        }
    }
}