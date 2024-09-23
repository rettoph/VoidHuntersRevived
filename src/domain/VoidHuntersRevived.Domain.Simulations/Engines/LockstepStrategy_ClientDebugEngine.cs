using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [StrategyFilter<LockstepStrategy_Client>]
    [SequenceGroup<EngineSequence>(EngineSequence.Group03)]
    [SequenceGroup<DrawSequence>(DrawSequence.Draw)]
    internal class LockstepStrategy_ClientDebugEngine : StrategyEngine<LockstepStrategy_Client>, ISimpleDebugEngine
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

        public LockstepStrategy_ClientDebugEngine()
        {
            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Tick, () => this.Strategy.CurrentTick.Id.ToString("#,###,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Step, () => $"{this.Strategy.StepsSinceTick}/{this.Strategy.StepsPerTick}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), BufferHead, () => $"{(this.Strategy._ticks.Head?.Id.ToString()) ?? "null"}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), BufferTail, () => $"{(this.Strategy._ticks.Tail?.Id.ToString()) ?? "null"}"),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), BufferCount, () => this.Strategy._ticks.Count.ToString("#,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), BufferCount, () => this.Strategy._ticks.Count.ToString("#,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), TimeSinceStep, () => this.Strategy.TimeSinceStep.ToString(@"hh\:mm\:ss\.FFFFFFF")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), StepTimespan, () => this.Strategy.StepTimeSpan.ToString(@"hh\:mm\:ss\.FFFFFFF")),
            };
        }
    }
}
