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
    [StrategyFilter<LockstepStrategy_Server>]
    [SequenceGroup<DrawSequence>(DrawSequence.Draw)]
    internal class LockstepStrategy_ServerDebugEngine : StrategyEngine<LockstepStrategy_Server>, ISimpleDebugEngine
    {
        public const string Time = nameof(Time);
        public const string Tick = nameof(Tick);
        public const string Step = nameof(Step);

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public LockstepStrategy_ServerDebugEngine()
        {
            this.Lines = new[]
            {
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Tick, () => this.Strategy.CurrentTick.Id.ToString("#,###,##0")),
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Step, () => $"{this.Strategy.StepsSinceTick}/{this.Strategy.StepsPerTick}"),
            };
        }
    }
}
