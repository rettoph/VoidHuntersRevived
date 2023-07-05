using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public class EntityData
    {
        public readonly Guid Id;
        public readonly byte[] Bytes;

        internal EntityData(byte[] bytes)
        {
            Id = Guid.NewGuid();
            Bytes = bytes;
        }
    }
}
