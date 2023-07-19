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
        public readonly bool Confirmed;

        public EntityReaderState(EntityData data, VhId seed, int position, bool confirmed)
        {
            Data = data;
            Seed = seed;
            Position = position;
            Confirmed = confirmed;
        }
    }
}
