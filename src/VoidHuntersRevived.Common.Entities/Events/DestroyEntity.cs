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
        public static readonly ParallelKey Noise = ParallelKey.From<DestroyEntity>();

        public readonly ParallelKey Key;
        public readonly bool Backup;

        public DestroyEntity(ParallelKey entity, bool backup)
        {
            this.Key = entity;
            this.Backup = backup;
        }
    }
}
