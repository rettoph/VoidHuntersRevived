using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [StrategyFilter<LockstepStrategy_Server>]
    internal class LockstepSimulation_ServerDebugEngine : StrategyEngine<LockstepStrategy_Server>, ISimpleDebugEngine
    {
        public const string Time = nameof(Time);
        public const string Tick = nameof(Tick);
        public const string Step = nameof(Step);

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public LockstepSimulation_ServerDebugEngine()
        {
            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Tick, () => this.Simulation.CurrentTick.Id.ToString("#,###,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Step, () => $"{this.Simulation.StepsSinceTick}/{this.Simulation.StepsPerTick}"),
            };
        }
    }
}
