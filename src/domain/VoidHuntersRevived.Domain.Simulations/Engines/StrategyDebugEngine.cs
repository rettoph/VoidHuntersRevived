using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Enums;
using VoidHuntersRevived.Domain.Entities.Common.Engines;
using VoidHuntersRevived.Domain.Simulations.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Engines;

namespace VoidHuntersRevived.Domain.Simulations.Engines
{
    [AutoLoad]
    [StrategyFilter<Strategy>]
    [Sequence<DrawSequence>(DrawSequence.PreDraw)]
    internal class StrategyDebugEngine : StrategyEngine<Strategy>, ISimpleDebugEngine
    {
        public const string Time = nameof(Time);

        public ISimpleDebugEngine.SimpleDebugLine[] Lines { get; }

        public StrategyDebugEngine()
        {
            this.Lines = [
                new ISimpleDebugEngine.SimpleDebugLine(nameof(IStrategy), Time, () => TimeSpan.FromSeconds((float)Strategy.CurrentStep.TotalTime).ToString(@"hh\:mm\:ss\.FFFFFFF").PadRight(16, '0'))
            ];
        }
    }
}
