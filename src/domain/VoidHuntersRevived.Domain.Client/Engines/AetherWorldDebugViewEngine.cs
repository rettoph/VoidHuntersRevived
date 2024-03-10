using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui;
using Guppy.Game.ImGui.Services;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Domain.Client.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    internal class AetherWorldDebugViewEngine : BasicEngine, IDebugEngine, IStepEngine<GameTime>, IImGuiComponent
    {
        public string? Group => typeof(World).Name;

        public string name { get; } = nameof(AetherWorldDebugViewEngine);

        private readonly ISimulation _simulation;
        private readonly IGuppy _guppy;
        private readonly IImGui _imgui;
        private readonly IImGuiObjectExplorerService _objectExplorer;
        private readonly World _world;
        private readonly DebugView _debug;
        private readonly Camera2D _camera;
        private bool _debugViewEnabled;
        private bool _aetherExplorerEnabled;
        private string _filter;

        public AetherWorldDebugViewEngine(
            ISimulation simulation,
            IGuppy guppy,
            IImGui imgui,
            IImGuiObjectExplorerService objectExplorer,
            World world,
            GraphicsDevice graphics,
            IResourceProvider resources,
            Camera2D camera)
        {
            _simulation = simulation;
            _guppy = guppy;
            _imgui = imgui;
            _objectExplorer = objectExplorer;
            _world = world;
            _debug = new DebugView(world);
            _camera = camera;
            _debug.LoadContent(graphics, resources.Get(Resources.SpriteFonts.Default));
            _filter = string.Empty;
        }

        public void Step(in GameTime param)
        {
            if (_debugViewEnabled == false)
            {
                return;
            }

            _debug.RenderDebugData(_camera.Projection, _camera.View);
        }

        public void DrawImGui(GameTime gameTime)
        {
            if (_aetherExplorerEnabled == false)
            {
                return;
            }

            _imgui.Begin($"Aether Explorer - {_simulation.Type}, {_guppy.Name} {_guppy.Id}", ref _aetherExplorerEnabled);

            _imgui.InputText("Filter", ref _filter, 255);

            using (_imgui.ApplyID(nameof(World.BodyList)))
            {
                _objectExplorer.DrawObjectExplorer(_world.BodyList, _filter, 8, new HashSet<object>() { _world });
            }

            using (_imgui.ApplyID(nameof(World.ContactManager)))
            {
                _objectExplorer.DrawObjectExplorer(_world.ContactManager, _filter, 8);
            }

            _imgui.End();
        }

        public void RenderDebugInfo(GameTime gameTime)
        {
            var buttonStyle = _debugViewEnabled ? Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonRed : Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonGreen;

            using (_imgui.Apply(buttonStyle))
            {
                if (_imgui.Button($"{(_debugViewEnabled ? "Disable" : "Enable")} DebugView"))
                {
                    _debugViewEnabled = !_debugViewEnabled;
                }
            }

            buttonStyle = _aetherExplorerEnabled ? Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonRed : Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonGreen;

            using (_imgui.Apply(buttonStyle))
            {
                if (_imgui.Button($"{(_aetherExplorerEnabled ? "Disable" : "Enable")} Aether Explorer"))
                {
                    _aetherExplorerEnabled = !_aetherExplorerEnabled;
                }
            }
        }
    }
}
