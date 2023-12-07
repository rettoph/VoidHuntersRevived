using Autofac;
using Guppy;
using Guppy.Attributes;
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
        private readonly ISimulationService _simulations;
        private (Simulation, Dictionary<string, ISimpleDebugEngine.SimpleDebugLine[]>, int)[] _data;
        private readonly IImGui _imgui;

        public SimulationDebugComponent(
            IImGui imgui, 
            ISimulationService simulations)
        {
            _imgui = imgui;
            _simulations = simulations;
            _data = Array.Empty<(Simulation, Dictionary<string, ISimpleDebugEngine.SimpleDebugLine[]>, int)>();
        }

        public override void Initialize(IGuppy guppy)
        {
            base.Initialize(guppy);

            _data = _simulations.Instances.Select(x => (
                (x as Simulation)!, 
                x.Scope.Resolve<IEngineService>().OfType<ISimpleDebugEngine>()
                    .Sequence(DrawSequence.Draw)
                    .SelectMany(x => x.Lines)
                    .GroupBy(x => x.Group)
                    .ToDictionary(x => x.Key, x => x.ToArray()),
                 x.Scope.Resolve<IEngineService>().OfType<ISimpleDebugEngine>()
                    .SelectMany(x => x.Lines)
                    .Max(x => x.Title.Length)
            )).ToArray();
        }

        public void RenderDebugInfo(GameTime gameTime)
        {
            foreach(var (simulation, groupedLines, padLeft) in _data)
            {
                _imgui.BeginChild($"{simulation.Type}", Vector2.Zero, ImGuiChildFlags.AlwaysAutoResize | ImGuiChildFlags.AutoResizeY);

                _imgui.Text($"Simulation: {simulation.Type}");

                _imgui.Indent();

                foreach(var (group, lines) in groupedLines)
                {
                    foreach (ISimpleDebugEngine.SimpleDebugLine line in lines)
                    {
                        _imgui.Text($"{line.Title.PadLeft(padLeft, ' ')}: ");
                        _imgui.SameLine();
                        _imgui.TextColored(Color.Cyan.ToVector4(), line.Value());
                    }

                    _imgui.NewLine();
                }


                _imgui.Unindent();

                _imgui.EndChild();
            }
        }
    }
}
