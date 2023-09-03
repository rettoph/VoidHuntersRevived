using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public struct EntityReaderState
    {
        public readonly EntityData Data;
        public readonly VhId Seed;
        public readonly int Position;
        public readonly VhId Injection;

        public EntityReaderState(EntityData data, VhId seed, int position, VhId injection)
        {
            Data = data;
            Seed = seed;
            Position = position;
            Injection = injection;
        }
    }
}
