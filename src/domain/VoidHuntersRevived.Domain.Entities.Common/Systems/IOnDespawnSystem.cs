using Guppy.Core.Common.Attributes;
using Guppy.Game.Common.Systems;
using Svelto.ECS;
using VoidHuntersRevived.Common;
using VoidHuntersRevived.Domain.Entities.Common.Enums;

namespace VoidHuntersRevived.Domain.Entities.Common.Systems
{
    public interface IOnDespawnSystem<T> : ISceneSystem
        where T : unmanaged, IEntityComponent
    {
        [RequireSequenceGroup<OnDespawnSequenceGroupEnum>]
        void OnDespawn(VhId sourceEventId, IEntityTemplate entityTemplate, ref Entity<T> entity);
    }

    public interface IOnDespawnSystem<T1, T2> : ISceneSystem
        where T1 : unmanaged, IEntityComponent
        where T2 : unmanaged, IEntityComponent
    {
        [RequireSequenceGroup<OnDespawnSequenceGroupEnum>]
        void OnDespawn(VhId sourceEventId, IEntityTemplate entityTemplate, ref Entity<T1, T2> entity);
    }
}