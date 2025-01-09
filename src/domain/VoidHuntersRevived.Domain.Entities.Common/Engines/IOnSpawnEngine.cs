using Guppy.Core.Common.Attributes;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Engines
{
    public interface IOnSpawnEngine<T>
        where T : unmanaged, IEntityComponent
    {
        [RequireSequenceGroup<OnSpawnSequenceGroupEnum>]
        void OnSpawn(VhId sourceEventId, IEntityTemplate entityTemplate, ref Entity<T> entity);
    }

    public interface IOnSpawnEngine<T1, T2>
        where T1 : unmanaged, IEntityComponent
        where T2 : unmanaged, IEntityComponent
    {
        [RequireSequenceGroup<OnSpawnSequenceGroupEnum>]
        void OnSpawn(VhId sourceEventId, IEntityTemplate entityTemplate, ref Entity<T1, T2> entity);
    }
}