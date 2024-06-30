using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [SimulationFilter<LockstepStrategy_Client>]
    internal class LockstepSimulation_ClientDebugEngine : BasicEngine<LockstepStrategy_Client>, ISimpleDebugEngine
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
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Tick, () => this.Simulation.CurrentTick.Id.ToString("#,###,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Step, () => $"{this.Simulation.stepsSinceTick}/{this.Simulation.stepsPerTick}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), BufferHead, () => $"{(this.Simulation._ticks.Head?.Id.ToString()) ?? "null"}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), BufferTail, () => $"{(this.Simulation._ticks.Tail?.Id.ToString()) ?? "null"}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), BufferCount, () => this.Simulation._ticks.Count.ToString("#,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), BufferCount, () => this.Simulation._ticks.Count.ToString("#,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), TimeSinceStep, () => this.Simulation.timeSinceStep.ToString(@"hh\:mm\:ss\.FFFFFFF")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), StepTimespan, () => this.Simulation.stepTimeSpan.ToString(@"hh\:mm\:ss\.FFFFFFF")),
            };
        }
    }
}
