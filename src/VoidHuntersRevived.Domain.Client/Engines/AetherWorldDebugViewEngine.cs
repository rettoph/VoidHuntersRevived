using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common.Client;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Client.Engines
{
    [AutoLoad]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    internal class AetherWorldDebugViewEngine : BasicEngine, IDrawDebuggerEngine, IStepEngine<GameTime>
    {
        public string Group => typeof(World).Name;

        public string name { get; } = nameof(AetherWorldDebugViewEngine);

        private readonly IImGui _imgui;
        private readonly DebugView _debug;
        private readonly Camera2D _camera;
        private bool _enabled;

        public AetherWorldDebugViewEngine(IImGui imgui, World world, GraphicsDevice graphics, IResourceProvider resources, Camera2D camera)
        {
            _imgui = imgui;
            _debug = new DebugView(world);
            _camera = camera;
            _debug.LoadContent(graphics, resources.Get(Resources.SpriteFonts.Default));
        }

        public void Step(in GameTime param)
        {
            if (_enabled == false)
            {
                return;
            }

            _debug.RenderDebugData(_camera.Projection, _camera.View);
        }

        public void DrawDebugger(GameTime gameTime)
        {
            var buttonStyle = _enabled ? Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonRed : Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonGreen;

            using (_imgui.Apply(buttonStyle))
            {
                if (_imgui.Button($"{(_enabled ? "Disable" : "Enable")} DebugView"))
                {
                    _enabled = !_enabled;
                }
            }
        }
    }
}
