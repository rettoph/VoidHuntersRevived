using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Simulations;

namespace VoidHuntersRevived.Common.Entities.Events
{
    public sealed class DestroyEntity
    {
        public static readonly EventId Noise = EventId.From<DestroyEntity>();

        public readonly EventId Key;
        public readonly bool Backup;

        public DestroyEntity(EventId entity, bool backup)
        {
            this.Key = entity;
            this.Backup = backup;
        }
    }
}
