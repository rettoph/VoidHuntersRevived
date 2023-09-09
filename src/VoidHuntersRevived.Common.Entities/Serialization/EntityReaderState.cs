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
        public readonly int Position;

        public EntityReaderState(EntityData data, int position)
        {
            Data = data;
            Position = position;
        }
    }
}
