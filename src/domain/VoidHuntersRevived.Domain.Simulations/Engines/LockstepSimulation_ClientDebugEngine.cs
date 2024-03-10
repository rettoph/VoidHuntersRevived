using Guppy.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [SimulationFilter<LockstepSimulation_Client>]
    internal class LockstepSimulation_ClientDebugEngine : BasicEngine<LockstepSimulation_Client>, ISimpleDebugEngine
    {
        public const string Time = nameof(Time);
        public const string Tick = nameof(Tick);
        public const string Step = nameof(Step);
        public const string BufferHead = nameof(BufferHead);
        public const string BufferTail = nameof(BufferTail);
        public const string BufferCount = nameof(BufferCount);
        public const string TimeSinceStep = nameof(TimeSinceStep);
        public const string StepTimespan = nameof(StepTimespan);

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public LockstepSimulation_ClientDebugEngine()
        {
            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), Tick, () => this.Simulation.CurrentTick.Id.ToString("#,###,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), Step, () => $"{this.Simulation.stepsSinceTick}/{this.Simulation.stepsPerTick}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), BufferHead, () => $"{(this.Simulation._ticks.Head?.Id.ToString()) ?? "null"}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), BufferTail, () => $"{(this.Simulation._ticks.Tail?.Id.ToString()) ?? "null"}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), BufferCount, () => this.Simulation._ticks.Count.ToString("#,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), BufferCount, () => this.Simulation._ticks.Count.ToString("#,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), TimeSinceStep, () => this.Simulation.timeSinceStep.ToString(@"hh\:mm\:ss\.FFFFFFF")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(ISimulation), StepTimespan, () => this.Simulation.stepTimeSpan.ToString(@"hh\:mm\:ss\.FFFFFFF")),
            };
        }
    }
}
