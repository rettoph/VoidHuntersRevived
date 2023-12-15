using Autofac;
using Guppy;
using Guppy.Attributes;
using Guppy.Commands.Messages;
using Guppy.Common;
using Guppy.Common.Attributes;
using Guppy.Common.Extensions;
using Guppy.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Enums;
using Guppy.Game.Components;
using Guppy.Game.ImGui;
using Guppy.Game.MonoGame.Utilities.Cameras;
using Guppy.Resources.Providers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using tainicom.Aether.Physics2D.Diagnostics;
using tainicom.Aether.Physics2D.Dynamics;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Client;
using VoidHuntersRevived.Common.Entities;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Physics;
using VoidHuntersRevived.Common.Pieces.Components;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Engines;
using VoidHuntersRevived.Common.Simulations.Lockstep;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations;

namespace VoidHuntersRevived.Domain.Client.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<IVoidHuntersGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    internal class SimulationDebugComponent : GuppyComponent, IDebugComponent
    {
        private class DebugEngineGroupRenderer
        {
            private readonly string _group;
            private int _titleLength;
            private ISimpleDebugEngine.SimpleDebugLine[] _lines;
            private IDrawDebuggerEngine[] _engines;

            public DebugEngineGroupRenderer(string group, ISimpleDebugEngine.SimpleDebugLine[] lines, IDrawDebuggerEngine[] engines)
            {
                _group = group;
                _titleLength = lines.Length == 0 ? 0 : lines.Max(x => x.Title.Length);
                _lines = lines;
                _engines = engines;
            }

            public void DrawImGui(IImGui imgui, GameTime gameTime)
            {
                foreach (ISimpleDebugEngine.SimpleDebugLine line in _lines)
                {
                    string title = line.Title.PadLeft(_titleLength, ' ') + ":";
                    string value = line.Value();

                    imgui.Text(title);
                    imgui.SameLine();
                    imgui.TextColored(Color.Cyan.ToVector4(), value);
                }

                foreach(var engine in _engines)
                {
                    engine.DrawDebugger(gameTime);
                }

                imgui.NewLine();
            }
        }
        private readonly ISimulationService _simulations;
        private (Simulation, Dictionary<string, DebugEngineGroupRenderer>)[] _data;
        private readonly IImGui _imgui;
        private IGuppy _guppy;

        public SimulationDebugComponent(
            IImGui imgui, 
            ISimulationService simulations)
        {
            _guppy = null!;
            _imgui = imgui;
            _simulations = simulations;
            _data = Array.Empty<(Simulation, Dictionary<string, DebugEngineGroupRenderer>)>();
        }

        public override void Initialize(IGuppy guppy)
        {
            base.Initialize(guppy);

            _guppy = guppy;

            _data = _simulations.Instances.Select(x => (
                (x as Simulation)!,
                new Dictionary<string, DebugEngineGroupRenderer>())).ToArray();

            foreach(var (simulation, renderers) in _data)
            {
                var simpleEngines = simulation.Scope.Resolve<IEngineService>().OfType<ISimpleDebugEngine>()
                    .Sequence(DrawSequence.Draw)
                    .SelectMany(x => x.Lines)
                    .GroupBy(x => x.Group)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                var engines = simulation.Scope.Resolve<IEngineService>().OfType<IDrawDebuggerEngine>()
                    .Sequence(DrawSequence.Draw)
                    .GroupBy(x => x.Group)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                var groups = simpleEngines.Select(x => x.Key).Concat(engines.Select(x => x.Key)).Distinct().ToArray();

                foreach(var group in groups)
                {
                    if(simpleEngines.TryGetValue(group, out var groupedSimpleEngines) == false)
                    {
                        groupedSimpleEngines = Array.Empty<ISimpleDebugEngine.SimpleDebugLine>();
                    }

                    if (engines.TryGetValue(group, out var groupedEngines) == false)
                    {
                        groupedEngines = Array.Empty<IDrawDebuggerEngine>();
                    }

                    renderers.Add(group, new DebugEngineGroupRenderer(group, groupedSimpleEngines, groupedEngines));
                }
            }
        }

        public void RenderDebugInfo(GameTime gameTime)
        {
            _imgui.PushID($"#Debugger#{_guppy.ToString()}");
            foreach (var (simulation, renderers) in _data)
            {
                _imgui.BeginChild($"{simulation.Type}", Vector2.Zero, ImGuiChildFlags.AlwaysAutoResize | ImGuiChildFlags.AutoResizeY);

                _imgui.Text($"Simulation: {simulation.Type}");

                _imgui.Indent();

                foreach(var (group, renderer) in renderers)
                {
                    renderer.DrawImGui(_imgui, gameTime);
                }

                _imgui.Unindent();

                _imgui.EndChild();
            }
            _imgui.PopID();
        }
    }
}
