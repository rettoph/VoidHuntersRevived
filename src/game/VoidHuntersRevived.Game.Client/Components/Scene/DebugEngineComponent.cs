using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Guppy.Game.Components;
using Guppy.Game.ImGui.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    [AutoLoad]
    [SceneFilter<IVoidHuntersGameScene>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    internal class DebugEngineComponent : SceneComponent, IDebugComponent
    {
        private class DebugEngineGroupRenderer
        {
            private readonly string _group;
            private int _titleLength;
            private ISimpleDebugEngine.SimpleDebugLine[] _lines;
            private IDebugEngine[] _engines;

            public DebugEngineGroupRenderer(string group, ISimpleDebugEngine.SimpleDebugLine[] lines, IDebugEngine[] engines)
            {
                _group = group;
                _titleLength = lines.Length == 0 ? 0 : lines.Max(x => x.Title.Length);
                _lines = lines;
                _engines = engines;
            }

            public void DrawImGui(IImGui imgui, GameTime gameTime)
            {
                if (imgui.CollapsingHeader(_group))
                {
                    imgui.Indent();

                    foreach (ISimpleDebugEngine.SimpleDebugLine line in _lines)
                    {
                        string title = line.Title.PadLeft(_titleLength, ' ') + ":";
                        string value = line.Value();

                        imgui.Text(title);
                        imgui.SameLine();
                        imgui.TextColored(Color.Cyan.ToVector4(), value);
                    }

                    foreach (var engine in _engines)
                    {
                        engine.RenderDebugInfo(gameTime);
                    }

                    imgui.Unindent();
                }
            }
        }
        private readonly ISimulationService _simulations;
        private (IStrategy, Dictionary<string, DebugEngineGroupRenderer>)[] _data;
        private readonly IImGui _imgui;
        private readonly IScene _scene;

        public DebugEngineComponent(
            IScene scene,
            IImGui imgui,
            ISimulationService simulations)
        {
            _scene = scene;
            _imgui = imgui;
            _simulations = simulations;
            _data = Array.Empty<(IStrategy, Dictionary<string, DebugEngineGroupRenderer>)>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _data = _simulations.Instances.SelectMany(x => x.Strategies).Select(x => (
                (x as IStrategy)!,
                new Dictionary<string, DebugEngineGroupRenderer>())).ToArray();

            foreach (var (simulation, renderers) in _data)
            {
                var simpleEngines = simulation.Engines.OfType<ISimpleDebugEngine>()
                    .Sequence(DrawSequence.Draw)
                    .SelectMany(x => x.Lines)
                    .GroupBy(x => x.Group)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                var engines = simulation.Engines.OfType<IDebugEngine>()
                    .Where(x => x.Group is not null)
                    .Sequence(DrawSequence.Draw)
                    .GroupBy(x => x.Group!)
                    .ToDictionary(x => x.Key, x => x.ToArray());

                var groups = simpleEngines.Select(x => x.Key).Concat(engines.Select(x => x.Key)).Distinct().ToArray();

                foreach (var group in groups)
                {
                    if (simpleEngines.TryGetValue(group, out var groupedSimpleEngines) == false)
                    {
                        groupedSimpleEngines = Array.Empty<ISimpleDebugEngine.SimpleDebugLine>();
                    }

                    if (engines.TryGetValue(group, out var groupedEngines) == false)
                    {
                        groupedEngines = Array.Empty<IDebugEngine>();
                    }

                    renderers.Add(group, new DebugEngineGroupRenderer(group, groupedSimpleEngines, groupedEngines));
                }
            }
        }

        public void RenderDebugInfo(GameTime gameTime)
        {
            _imgui.PushID($"#Debugger#{_scene.ToString()}");
            foreach (var (simulation, renderers) in _data)
            {
                _imgui.BeginChild($"{simulation.Type}", Vector2.Zero, ImGuiChildFlags.AlwaysAutoResize | ImGuiChildFlags.AutoResizeY);

                _imgui.Text($"Simulation: {simulation.Type}");

                foreach (var (group, renderer) in renderers)
                {
                    renderer.DrawImGui(_imgui, gameTime);
                }

                _imgui.NewLine();

                _imgui.EndChild();
            }
            _imgui.PopID();
        }
    }
}
