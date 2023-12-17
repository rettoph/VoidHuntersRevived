﻿using Guppy;
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
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;

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
        private readonly World _world;
        private readonly DebugView _debug;
        private readonly Camera2D _camera;
        private bool _debugViewEnabled;
        private bool _aetherViewerEnabled;

        public AetherWorldDebugViewEngine(
            ISimulation simulation,
            IGuppy guppy,
            IImGui imgui, 
            World world, 
            GraphicsDevice graphics, 
            IResourceProvider resources, 
            Camera2D camera)
        {
            _simulation = simulation;
            _guppy = guppy;
            _imgui = imgui;
            _world = world;
            _debug = new DebugView(world);
            _camera = camera;
            _debug.LoadContent(graphics, resources.Get(Resources.SpriteFonts.Default));
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
            if(_aetherViewerEnabled == false)
            {
                return;
            }

            _imgui.Begin($"Aether Viewer - {_simulation.Type}, {_guppy.Name} {_guppy.Id}", ref _aetherViewerEnabled);
            _imgui.ObjectViewer(_world, "", 7);
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

            buttonStyle = _aetherViewerEnabled ? Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonRed : Guppy.Game.MonoGame.Resources.ImGuiStyles.ButtonGreen;

            using (_imgui.Apply(buttonStyle))
            {
                if (_imgui.Button($"{(_aetherViewerEnabled ? "Disable" : "Enable")} Aether Viewer"))
                {
                    _aetherViewerEnabled = !_aetherViewerEnabled;
                }
            }
        }
    }
}
