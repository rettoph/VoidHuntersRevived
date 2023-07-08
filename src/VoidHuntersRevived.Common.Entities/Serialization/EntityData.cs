using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public class EntityData
    {
        public readonly VhId Id;
        public readonly byte[] Bytes;

        internal EntityData(VhId id, byte[] bytes)
        {
            Id = id;
            Bytes = bytes;
        }
    }
}
