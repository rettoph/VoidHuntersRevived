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

    public interface IOnDespawnEngine<T1, T2>
        where T1 : unmanaged, IEntityComponent
        where T2 : unmanaged, IEntityComponent
    {
        [RequireSequenceGroup<OnDespawnSequenceGroupEnum>]
        void OnDespawn(VhId sourceEventId, IEntityTemplate entityTemplate, ref Entity<T1, T2> entity);
    }
}