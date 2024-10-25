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
        void OnSpawn(VhId sourceEventId, IEntityTemplate entityTemplate, EntityId id, ref T component, in GroupIndex groupIndex);
    }
}
