using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IOnTickSystem : IEngine
    {
        [RequireSequenceGroup<OnTickSequenceGroupEnum>]
        void OnTick(Tick tick);
    }
}