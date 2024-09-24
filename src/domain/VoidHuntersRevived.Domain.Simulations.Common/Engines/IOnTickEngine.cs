using Guppy.Core.Common.Attributes;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IOnTickEngine
    {
        [RequireSequenceGroup<OnTickSequenceGroup>]
        void OnTick(Tick tick);
    }
}
