using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Common.Engines
{
    public interface IOnTickEngine : IEngine
    {
        [RequireSequenceGroup<OnTickSequenceGroup>]
        void OnTick(Tick tick);
    }
}