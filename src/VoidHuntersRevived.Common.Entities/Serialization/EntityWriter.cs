using Svelto.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VoidHuntersRevived.Common.Entities.Serialization
{
    public sealed class EntityWriter : BinaryWriter
    {
        public EntityWriter(EntityData data) : base(data, Encoding.UTF8, true)
        {
        }

        public unsafe void WriteUnmanaged<T>(T value)
            where T : unmanaged
        {
            byte* pBytes = (byte*)&value;
            var span = new ReadOnlySpan<byte>(pBytes, sizeof(T));

            this.Write(span);
        }

        public void Write(VhId vhid)
        {
            this.WriteUnmanaged(vhid);
        }
    }
}
