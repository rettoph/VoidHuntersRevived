using Guppy.Core.Common.Attributes;
using Guppy.Core.Resources.Common;
using Guppy.Core.Resources.Common.Services;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Game.Graphics.Common;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Enums;
using Guppy.Game.ImGui.Common.Extensions;
using Guppy.Game.ImGui.Common.Services;
using Guppy.Game.ImGui.Common.Styling;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Game.Client.Engines.Debugging
{
    [AutoLoad]
    internal class AetherDebugEngine : StrategyEngine, IOnDrawEngine, IImGuiComponent, IOnDebugEngine
    {
        public string? Group => typeof(World).Name;

        private readonly IStrategy _strategy;
        private readonly IScene _scene;
        private readonly IImGui _imgui;
        private readonly IImGuiObjectExplorerService _objectExplorer;
        private readonly World _world;
        private readonly DebugView _debug;
        private readonly ICamera2D _camera;
        private bool _debugViewEnabled;
        private bool _aetherExplorerEnabled;
        private string _filter;
        private readonly Resource<ImStyle> _buttonRedStyle;
        private readonly Resource<ImStyle> _buttonGreenStyle;

        public AetherDebugEngine(
            IStrategy strategy,
            IScene scene,
            IImGui imgui,
            IImGuiObjectExplorerService objectExplorer,
            IResourceService resourceService,
            World world,
            GraphicsDevice graphics,
            ICamera2D camera)
        {
            _strategy = strategy;
            _scene = scene;
            _imgui = imgui;
            _objectExplorer = objectExplorer;
            _world = world;
            _debug = new DebugView(world);
            _camera = camera;
            _debug.LoadContent(graphics, resourceService.Get(Resources.SpriteFonts.Default));
            _filter = string.Empty;

            _buttonRedStyle = resourceService.Get(Resources.ImGuiStyles.ButtonRed);
            _buttonGreenStyle = resourceService.Get(Resources.ImGuiStyles.ButtonGreen);
        }

        [SequenceGroup<OnDrawSequenceGroup>(OnDrawSequenceGroup.Draw)]
        public void OnDraw(GameTime gameTime)
        {
            if (_debugViewEnabled == false)
            {
                return;
            }

            _debug.RenderDebugData(_camera.Projection, _camera.View, _camera.World);
        }

        [SequenceGroup<DebugSequenceGroup>("Aether")]
        public void OnDebug(GameTime gameTime)
        {
            _imgui.KeyValue("Bodies", _world.BodyList.Count.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());
            _imgui.KeyValue("Contacts", _world.ContactCount.ToString("#,###,##0"), valueColor: Color.Cyan.ToVector4());


            Resource<ImStyle> buttonStyle = _debugViewEnabled ? _buttonRedStyle : _buttonRedStyle;

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

        [SequenceGroup<ImGuiSequenceGroup>(ImGuiSequenceGroup.PostDraw)]
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
                _objectExplorer.DrawObjectExplorer(_world.BodyList, _filter, 8, [_world]);
            }

            using (_imgui.ApplyID(nameof(World.ContactManager)))
            {
                _objectExplorer.DrawObjectExplorer(_world.ContactManager, _filter, 8);
            }

            _imgui.End();
        }
    }
}
