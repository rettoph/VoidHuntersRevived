using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
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
