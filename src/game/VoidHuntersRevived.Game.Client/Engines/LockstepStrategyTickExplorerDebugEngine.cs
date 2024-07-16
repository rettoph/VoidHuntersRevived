using Guppy.Core.Common.Attributes;
using Guppy.Game.Common;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [StrategyFilter<ILockstepStrategy>]
    internal class LockstepStrategyTickExplorerDebugEngine : StrategyEngine<ILockstepStrategy>, IDebugEngine, IImGuiComponent
    {
        public string? Group => nameof(IStrategy);

        private readonly IImGui _imgui;
        private readonly IImGuiObjectExplorerService _objectExplorer;
        private readonly IScene _scene;
        private bool _historyViewerEnabled;
        private string _filter;

        public LockstepStrategyTickExplorerDebugEngine(
            IImGui imgui,
            IImGuiObjectExplorerService objectExplorer,
            IScene guppy)
        {
            _imgui = imgui;
            _objectExplorer = objectExplorer;
            _scene = guppy;
            _filter = string.Empty;
        }

        public void DrawImGui(GameTime gameTime)
        {
            if (_historyViewerEnabled == false)
            {
                return;
            }

            _imgui.Begin($"Tick History Explorer - {this.Simulation.Type}, {_scene.Name} {_scene.Id}", ref _historyViewerEnabled);
            _imgui.InputText("Filter", ref _filter, 255);

            using (_imgui.ApplyID(nameof(ILockstepStrategy.History)))
            {
                _objectExplorer.DrawObjectExplorer(this.Simulation.History, _filter, 8);
            }

            _imgui.End();
        }

        public void RenderDebugInfo(GameTime gameTime)
        {
            var buttonStyle = _historyViewerEnabled ? Guppy.Game.MonoGame.Common.Resources.ImGuiStyles.ButtonRed : Guppy.Game.MonoGame.Common.Resources.ImGuiStyles.ButtonGreen;

            using (_imgui.Apply(buttonStyle))
            {
                if (_imgui.Button($"{(_historyViewerEnabled ? "Disable" : "Enable")} Tick History Explorer"))
                {
                    _historyViewerEnabled = !_historyViewerEnabled;
                }
            }
        }
    }
}
