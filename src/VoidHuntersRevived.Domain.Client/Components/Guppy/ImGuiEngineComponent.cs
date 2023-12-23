using Autofac;
using Guppy;
using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Common.Extensions;
using Guppy.Enums;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui;
using Microsoft.Xna.Framework;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Common.Entities.Services;
using VoidHuntersRevived.Common.Simulations.Services;
using VoidHuntersRevived.Domain.Simulations;

namespace VoidHuntersRevived.Domain.Client.Components.Guppy
{
    [AutoLoad]
    [GuppyFilter<IVoidHuntersGameGuppy>]
    [Sequence<InitializeSequence>(InitializeSequence.PostInitialize)]
    [Sequence<DrawSequence>(DrawSequence.PostDraw)]
    internal class ImGuiEngineComponent : GuppyComponent, IImGuiComponent
    {
        private IGuppy _guppy;
        private readonly IImGui _imgui;
        private (Simulation, IImGuiComponent[])[] _data;
        private readonly ISimulationService _simulations;

        public ImGuiEngineComponent(
            IImGui imgui,
            ISimulationService simulations)
        {
            _guppy = null!;
            _imgui = imgui;
            _simulations = simulations;
            _data = Array.Empty<(Simulation, IImGuiComponent[])>();
        }

        public override void Initialize(IGuppy guppy)
        {
            base.Initialize(guppy);

            _guppy = guppy;
            _data = _simulations.Instances.Select(x => (
                (x as Simulation)!,
                x.Scope.Resolve<IEngineService>().OfType<IImGuiComponent>().Sequence(DrawSequence.Draw).ToArray()
            )).ToArray();
        }

        public void DrawImGui(GameTime gameTime)
        {
            foreach (var (simulation, engines) in _data)
            {
                _imgui.PushID($"#{_guppy.Id}#{simulation.Type}#{nameof(IImGuiComponent)}s");
                foreach (var engine in engines)
                {
                    engine.DrawImGui(gameTime);
                }
                _imgui.PopID();
            }
        }
    }
}
