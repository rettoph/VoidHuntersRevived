using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using VoidHuntersRevived.Domain.Simulations.Common.Enums;

namespace VoidHuntersRevived.Domain.Simulations.Common.Systems
{
    public interface ITickSystem : ISceneSystem
    {
        [RequireSequenceGroup<TickSequenceGroupEnum>]
        void Tick(Tick tick);
    }
}