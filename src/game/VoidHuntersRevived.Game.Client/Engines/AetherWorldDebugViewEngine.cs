using Guppy.Core.Common.Attributes;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Services;
using Guppy.Game.ImGui.Common.Styling;
using Guppy.Game.MonoGame.Common.Utilities.Cameras;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Game.Client.Engines
{
    [AutoLoad]
    [Sequence<EngineSequence>(EngineSequence.Group03)]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    internal class AetherWorldDebugViewEngine : StrategyEngine, IDebugEngine, IStepEngine<GameTime>, IImGuiComponent
    {
        public string? Group => typeof(World).Name;

        public string name { get; } = nameof(AetherWorldDebugViewEngine);

        private readonly IStrategy _strategy;
        private readonly IScene _scene;
        private readonly IImGui _imgui;
        private readonly IImGuiObjectExplorerService _objectExplorer;
        private readonly World _world;
        private readonly DebugView _debug;
        private readonly Camera2D _camera;
        private bool _debugViewEnabled;
        private bool _aetherExplorerEnabled;
        private string _filter;
        private ResourceValue<ImStyle> _buttonRedStyle;
        private ResourceValue<ImStyle> _buttonGreenStyle;

        public AetherWorldDebugViewEngine(
            IStrategy strategy,
            IScene scene,
            IImGui imgui,
            IImGuiObjectExplorerService objectExplorer,
            IResourceService resourceService,
            World world,
            GraphicsDevice graphics,
            Camera2D camera)
        {
            _strategy = strategy;
            _scene = scene;
            _imgui = imgui;
            _objectExplorer = objectExplorer;
            _world = world;
            _debug = new DebugView(world);
            _camera = camera;
            _debug.LoadContent(graphics, resourceService.GetValue(Resources.SpriteFonts.Default));
            _filter = string.Empty;

            _buttonRedStyle = resourceService.GetValue(Guppy.Game.MonoGame.Common.Resources.ImGuiStyles.ButtonRed);
            _buttonGreenStyle = resourceService.GetValue(Guppy.Game.MonoGame.Common.Resources.ImGuiStyles.ButtonGreen);
        }

        public void Step(in GameTime param)
        {
            if (_debugViewEnabled == false)
            {
                return;
            }

            _debug.RenderDebugData(_camera.Projection, _camera.View, _camera.World);
        }

        public void DrawImGui(GameTime gameTime)
        {
            if (_aetherExplorerEnabled == false)
            {
                return;
            }

            _imgui.Begin($"Aether Explorer - {_strategy.Type}, {_scene.Name} {_scene.Id}", ref _aetherExplorerEnabled);

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
            ResourceValue<ImStyle> buttonStyle = _debugViewEnabled ? _buttonRedStyle : _buttonRedStyle;

            using (_imgui.Apply(buttonStyle))
            {
                if (_imgui.Button($"{(_debugViewEnabled ? "Disable" : "Enable")} DebugView"))
                {
                    _debugViewEnabled = !_debugViewEnabled;
                }
            }

            buttonStyle = _aetherExplorerEnabled ? _buttonRedStyle : _buttonRedStyle;

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
