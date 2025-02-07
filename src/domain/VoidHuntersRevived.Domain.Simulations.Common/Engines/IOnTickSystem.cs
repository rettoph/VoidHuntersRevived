using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;
using VoidHuntersRevived.Domain.Simulations.Common.Lockstep;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface IOnTickSystem : ISceneSystem
    {
        [RequireSequenceGroup<OnTickSequenceGroupEnum>]
        void OnTick(Tick tick);
    }
}