using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.ImGui.Common;
using Guppy.Game.ImGui.Common.Enums;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Client.Components.Scene
{
    //[AutoLoad]
    [SceneFilter<IVoidHuntersGameScene>]
    [SequenceGroup<InitializeSequence>(InitializeSequence.PostInitialize)]
    internal class ImGuiEngineComponent : SceneComponent, IImGuiComponent
    {
        private readonly IScene _scene;
        private readonly IImGui _imgui;
        private (IStrategy, IImGuiComponent[])[] _data;
        private readonly ISimulationService _simulationService;

        public ImGuiEngineComponent(
            IScene scene,
            IImGui imgui,
            ISimulationService simulationService)
        {
            _scene = scene;
            _imgui = imgui;
            _simulationService = simulationService;
            _data = Array.Empty<(IStrategy, IImGuiComponent[])>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _data = _simulationService.Instances.SelectMany(x => x.Strategies).Select(x => (
                (x as IStrategy)!,
                x.Engines.Sequence<IImGuiComponent, DrawImGuiSequenceGroup>().ToArray()
            )).ToArray();
        }

        [SequenceGroup<DrawImGuiSequenceGroup>(DrawImGuiSequenceGroup.PostDraw)]
        public void DrawImGui(GameTime gameTime)
        {
            foreach (var (simulation, engines) in _data)
            {
                _imgui.PushID($"#{_scene.Id}#{simulation.Type}#{nameof(IImGuiComponent)}s");
                foreach (var engine in engines)
                {
                    engine.DrawImGui(gameTime);
                }
                _imgui.PopID();
            }
        }
    }
}
