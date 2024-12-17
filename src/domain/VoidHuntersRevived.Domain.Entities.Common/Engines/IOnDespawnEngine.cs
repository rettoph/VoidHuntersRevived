using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Engines
{
    public interface IOnDespawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        [RequireSequenceGroup<OnDespawnSequenceGroupEnum>]
        void OnDespawn(VhId sourceEventId, IEntityTemplate entityTemplate, ref Entity<T> entity);
    }
}
