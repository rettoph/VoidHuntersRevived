using Guppy.Game.Common;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Services;

namespace VoidHuntersRevived.Domain.Simulations.Common
{
    public interface IStrategy : IScene, IDisposable
    {
        StrategyTypeEnum Type { get; }
        ISimulation Simulation { get; }

        IEventService Events { get; }
    }
}