using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui;
using VoidHuntersRevived.Common.Entities.Engines;
using VoidHuntersRevived.Common.Simulations;
using VoidHuntersRevived.Common.Simulations.Attributes;
using VoidHuntersRevived.Common.Simulations.Engines;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [SimulationFilter<Simulation>]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal class SimulationDebugComponent : BasicEngine<Simulation>, ISimpleDebugEngine
    {
        public const string Time = nameof(Time);

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public SimulationDebugComponent(IImGui imgui)
        {
            Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), Time, () => TimeSpan.FromSeconds((float)Simulation.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'))
            };
        }
    }
}
