using Guppy.Core.Common.Attributes;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Enums;
using Guppy.Game.ImGui.Common.Services;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Game.Client.Engines.Debugging
{
    [AutoLoad]
    [StrategyFilter<ILockstepStrategy>]
    internal class LockstepStrategyDebugEngine : StrategyEngine<ILockstepStrategy>, IImGuiComponent, IOnDebugEngine
    {
        public string? Group => nameof(IStrategy);

        private readonly IImGui _imgui;
        private readonly IImGuiObjectExplorerService _objectExplorer;
        private readonly IScene _scene;
        private bool _historyViewerEnabled;
        private string _filter;

        public LockstepStrategyDebugEngine(
            IImGui imgui,
            IImGuiObjectExplorerService objectExplorer,
            IScene guppy)
        {
            _imgui = imgui;
            _objectExplorer = objectExplorer;
            _scene = guppy;
            _filter = string.Empty;
        }

        [SequenceGroup<DebugSequenceGroup>("Strategy")]
        public void OnDebug(GameTime gameTime)
        {
            var buttonStyle = _historyViewerEnabled ? Guppy.Game.MonoGame.Common.Resources.ImGuiStyles.ButtonGreen : Guppy.Game.MonoGame.Common.Resources.ImGuiStyles.ButtonRed;

            using (_imgui.Apply(buttonStyle))
            {
                if (_imgui.Button($"{(_historyViewerEnabled ? "Disable" : "Enable")} Tick History Explorer"))
                {
                    _historyViewerEnabled = !_historyViewerEnabled;
                }
            }
        }

        [SequenceGroup<ImGuiSequenceGroup>(ImGuiSequenceGroup.Draw)]
        public void DrawImGui(GameTime gameTime)
        {
            if (_historyViewerEnabled == false)
            {
                return;
            }

            _imgui.Begin($"Tick History Explorer - {Strategy.Type}, {_scene.Name} {_scene.Id}", ref _historyViewerEnabled);
            _imgui.InputText("Filter", ref _filter, 255);

            using (_imgui.ApplyID(nameof(ILockstepStrategy.History)))
            {
                _objectExplorer.DrawObjectExplorer(Strategy.History, _filter, 8);
            }

            _imgui.End();
        }
    }
}
