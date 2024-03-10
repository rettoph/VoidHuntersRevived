using Guppy.Attributes;
using Guppy.Common.Attributes;
using Guppy.Game.Common.Enums;
using Guppy.Game.ImGui;
using Guppy.StateMachine;
using Guppy.StateMachine.Services;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [SimulationFilter<Simulation>]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal class SimulationDebugComponent : BasicEngine<Simulation>, ISimpleDebugEngine
    {
        private readonly IStateService _states;

        public const string Time = nameof(Time);

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public SimulationDebugComponent(IImGui imgui, IStateService states)
        {
            _states = states;

            this.Lines = new ISimpleDebugEngine.SimpleDebugLine[1 + states.GetAll().Count()];
            this.Lines[0] = new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), Time, () => TimeSpan.FromSeconds((float)Simulation.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'));

            int index = 1;
            foreach (IState state in _states.GetAll())
            {
                this.Lines[index++] = new ISimpleDebugEngine.SimpleDebugLine(nameof(IStateService), $"{state.Key.Type.FullName}<{state.Key.Value}>", () => state.Value?.ToString() ?? string.Empty);
            }
        }
    }
}
