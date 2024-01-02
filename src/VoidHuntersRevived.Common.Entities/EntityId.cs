using Svelto.ECS;
using VoidHuntersRevived.Common.Core;

namespace VoidHuntersRevived.Common.Entities
{
    public struct EntityId : IEntityComponent
    {
        public static readonly EntityId Empty = default;

        /// <summary>
        /// Svelto's EGID, non deterministic.
        /// </summary>
        public readonly EGID EGID;

        /// <summary>
        /// Determinstic internal id
        /// </summary>
        public readonly VhId VhId;

        public EntityId(EGID eGID, VhId vhId)
        {
            this.EGID = eGID;
            this.VhId = vhId;
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityId id &&
                   VhId.Equals(id.VhId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(VhId);
        }

        public static bool operator ==(EntityId id1, EntityId id2)
        {
            return id1.VhId.Value == id2.VhId.Value;
        }

        public static bool operator !=(EntityId id1, EntityId id2)
        {
            return id1.VhId.Value != id2.VhId.Value;
        }
    }
}
