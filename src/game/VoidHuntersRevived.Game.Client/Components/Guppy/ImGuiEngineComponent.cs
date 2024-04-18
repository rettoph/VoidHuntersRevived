using Autofac;
using Guppy.Core.Common.Attributes;
using Guppy.Core.Common.Extensions;
using Guppy.Engine.Common.Enums;
using Guppy.Game.Common;
using Guppy.Game.Common.Attributes;
using Guppy.Game.Common.Components;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui.Common;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Domain.Common;
using VoidHuntersRevived.Domain.Entities.Common.Services;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Game.Client.Components.Guppy
{
    [AutoLoad]
    [SceneFilter<IVoidHuntersGameScene>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    internal class ImGuiEngineComponent : SceneComponent, IImGuiComponent
    {
        private readonly IScene _scene;
        private readonly IImGui _imgui;
        private (ISimulation, IImGuiComponent[])[] _data;
        private readonly ISimulationService _simulations;

        public ImGuiEngineComponent(
            IScene scene,
            IImGui imgui,
            ISimulationService simulations)
        {
            _scene = scene;
            _imgui = imgui;
            _simulations = simulations;
            _data = Array.Empty<(ISimulation, IImGuiComponent[])>();
        }

        protected override void Initialize()
        {
            base.Initialize();

            _data = _simulations.Instances.Select(x => (
                (x as ISimulation)!,
                x.Scope.Resolve<IEngineService>().OfType<IImGuiComponent>().Sequence(DrawSequence.Draw).ToArray()
            )).ToArray();
        }

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
