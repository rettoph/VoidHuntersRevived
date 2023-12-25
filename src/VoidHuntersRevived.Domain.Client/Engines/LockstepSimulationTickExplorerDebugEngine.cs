using Guppy;
using Guppy.Attributes;
using Guppy.Game.ImGui;
using Guppy.Game.ImGui.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Client.Engines
{
    [AutoLoad]
    [SimulationFilter<LockstepSimulation>]
    internal class LockstepSimulationTickExplorerDebugEngine : BasicEngine<LockstepSimulation>, IDebugEngine, IImGuiComponent
    {
        public string? Group => nameof(ISimulation);

        private readonly IImGui _imgui;
        private readonly IImGuiObjectExplorerService _objectExplorer;
        private readonly IGuppy _guppy;
        private bool _historyViewerEnabled;
        private string _filter;

        public LockstepSimulationTickExplorerDebugEngine(
            IImGui imgui,
            IImGuiObjectExplorerService objectExplorer,
            IGuppy guppy)
        {
            _imgui = imgui;
            _objectExplorer = objectExplorer;
            _guppy = guppy;
            _filter = string.Empty;
        }

        public void DrawImGui(GameTime gameTime)
        {
            if (_historyViewerEnabled == false)
            {
                return;
            }

            _imgui.Begin($"Tick History Explorer - {this.Simulation.Type}, {_guppy.Name} {_guppy.Id}", ref _historyViewerEnabled);
            _imgui.InputText("Filter", ref _filter, 255);

            using (_imgui.ApplyID(nameof(LockstepSimulation.History)))
            {
                _objectExplorer.DrawObjectExplorer(this.Simulation.History, _filter, 8);
            }

            _imgui.End();
        }

        public void RenderDebugInfo(GameTime gameTime)
        {
            var buttonStyle = _historyViewerEnabled ? Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonRed : Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonGreen;

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
