using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [SimulationFilter<Strategy>]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal class SimulationDebugEngine : BasicEngine<Strategy>, ISimpleDebugEngine
    {
        public const string Time = nameof(Time);

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public SimulationDebugEngine()
        {
            this.Lines = [
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Time, () => TimeSpan.FromSeconds((float)Simulation.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'))
            ];
        }
    }
}
